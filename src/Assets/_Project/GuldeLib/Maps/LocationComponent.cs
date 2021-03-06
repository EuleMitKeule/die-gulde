using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Economy;
using GuldeLib.Entities;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    public class LocationComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Settings")]
        public Vector2Int EntryCell { get; set; }

        [ShowInInspector]
        [BoxGroup("Settings")]
        public GameObject MapPrefab { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public MapComponent ContainingMap { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public MapComponent AssociatedMap { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public List<ExchangeComponent> Exchanges => GetComponentsInChildren<ExchangeComponent>().ToList();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry => this.GetCachedComponent<EntityRegistryComponent>();

        public event EventHandler<EntityRegistryComponent.EntityEventArgs> EntitySpawned;
        public event EventHandler<EntityRegistryComponent.EntityEventArgs> EntityArrived;
        public event EventHandler<EntityRegistryComponent.EntityEventArgs> EntityLeft;
        public event EventHandler<MapEventArgs> ContainingMapChanged;

        void Awake()
        {
            this.Log("Location initializing");

            //TODO refactor to factory
            if (MapPrefab)
            {
                var mapObject = Instantiate(MapPrefab, ContainingMap.transform.parent);
                AssociatedMap = mapObject.GetComponent<MapComponent>();
            }
        }

        public void OnEntityRegistered(object sender, EntityRegistryComponent.EntityEventArgs e)
        {
            this.Log(e.Entity.Map
            ? $"Location registering {e.Entity}"
            : $"Location spawning {e.Entity}");

            e.Entity.SetLocation(this);

            if (!e.Entity.Map) e.Entity.SetCell(EntryCell);

            if (AssociatedMap) AssociatedMap.EntityRegistry.Register(e.Entity);

            if (!e.Entity.Map)
            {
                EntitySpawned?.Invoke(this, new EntityRegistryComponent.EntityEventArgs(e.Entity));
                return;
            }

            EntityArrived?.Invoke(this, new EntityRegistryComponent.EntityEventArgs(e.Entity));
        }

        public void OnEntityUnregistered(object sender, EntityRegistryComponent.EntityEventArgs e)
        {
            this.Log($"Location unregistering {e.Entity}");

            e.Entity.SetLocation(null);

            if (AssociatedMap)
            {
                this.Log($"Location relocating {e.Entity} from {AssociatedMap} to {ContainingMap}");

                AssociatedMap.EntityRegistry.Unregister(e.Entity);
                ContainingMap.EntityRegistry.Register(e.Entity);
            }

            EntityLeft?.Invoke(this, new EntityRegistryComponent.EntityEventArgs(e.Entity));
        }

        public void SetContainingMap(MapComponent map)
        {
            this.Log($"Location setting containing map to {map}");

            ContainingMap = map;

            ContainingMapChanged?.Invoke(this, new MapEventArgs(map));
        }

        public class MapEventArgs : EventArgs
        {
            public MapEventArgs(MapComponent map)
            {
                Map = map;
            }

            public MapComponent Map { get; }
        }
    }
}