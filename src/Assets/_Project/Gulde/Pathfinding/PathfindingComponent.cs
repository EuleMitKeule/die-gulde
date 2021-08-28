using System;
using System.Collections.Generic;
using Gulde.Entities;
using Gulde.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Pathfinding
{
    [ExecuteAlways]
    [RequireComponent(typeof(EntityComponent))]
    public class PathfindingComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        float Speed { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        float CellThreshold { get; set; }

        [OdinSerialize]
        [BoxGroup("Path")]
        public Queue<Vector3Int> Waypoints { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        EntityComponent EntityComponent { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Vector3 Position => transform.position;

        [ShowInInspector]
        [BoxGroup("Info")]
        Vector3Int CellPosition => Position.ToCell();

        [ShowInInspector]
        [BoxGroup("Info")]
        bool HasWaypoints => Waypoints != null && Waypoints.Count > 0;

        bool IsAtWaypoint => HasWaypoints && Position.DistanceTo(CurrentWaypoint) < CellThreshold;

        Vector3Int CurrentWaypoint => Waypoints.Peek();
        public WaitForDestinationReached WaitForDestinationReached => new WaitForDestinationReached(this);

        public event EventHandler<CellEventArgs> DestinationReached;

        void Awake()
        {
            EntityComponent = GetComponent<EntityComponent>();
        }

        void FixedUpdate()
        {
            if (!HasWaypoints) return;

            var direction = Position.DirectionTo(CurrentWaypoint);

            transform.position += direction * (Speed * Time.fixedDeltaTime);

            if (!IsAtWaypoint) return;

            var cell = Waypoints.Dequeue();

            if (HasWaypoints) return;

            DestinationReached?.Invoke(this, new CellEventArgs(cell));
        }

        public void SetDestination(Vector3Int destinationCell)
        {
            if (!Application.isPlaying)
            {
                transform.position = destinationCell.ToWorld();
                DestinationReached?.Invoke(this, new CellEventArgs(destinationCell));
                return;
            }

            Waypoints = Pathfinder.FindPath(CellPosition, destinationCell, EntityComponent.Map);

            if (Waypoints == null || Waypoints.Count == 0)
            {
                Debug.Log($"{name} couldn't find a path!");
                DestinationReached?.Invoke(this, new CellEventArgs(destinationCell));
            }
        }
    }

    public class WaitForDestinationReached : CustomYieldInstruction
    {
        public WaitForDestinationReached(PathfindingComponent pathfinding)
        {
            Pathfinding = pathfinding;
            Pathfinding.DestinationReached += OnDestinationReached;
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            IsDestinationReached = true;
        }

        PathfindingComponent Pathfinding { get; }
        bool IsDestinationReached { get; set; }
        public override bool keepWaiting => !IsDestinationReached && Pathfinding.Waypoints.Count != 0;
    }
}