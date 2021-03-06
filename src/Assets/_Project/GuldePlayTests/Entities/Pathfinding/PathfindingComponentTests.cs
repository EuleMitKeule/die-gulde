// using System.Collections;
// using GuldeLib.Builders;
// using GuldeLib.Cities;
// using GuldeLib.Entities;
// using GuldeLib.Pathfinding;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// namespace GuldePlayTests.Entities.Pathfinding
// {
//     public class PathfindingComponentTests
//     {
//         GameBuilder GameBuilder { get; set; }
//         CityBuilder CityBuilder { get; set; }
//         EntityBuilder EntityBuilder { get; set; }
//
//         GameObject CityObject => CityBuilder.CityObject;
//         CityComponent City => CityObject.GetComponent<CityComponent>();
//         GameObject EntityObject { get; set; }
//         EntityComponent Entity => EntityObject.GetComponent<EntityComponent>();
//         PathfinderComponent Pathfinding => EntityObject.GetComponent<PathfinderComponent>();
//
//         bool DestinationReachedFlag { get; set; }
//         bool DestinationChangedFlag { get; set; }
//
//         [UnitySetUp]
//         public IEnumerator Setup()
//         {
//             CityBuilder = A.City.WithSize(20, 20);
//             GameBuilder = A.Game.WithCity(CityBuilder);
//
//             yield return GameBuilder.Build();
//
//             EntityBuilder = A.Entity.WithName("entity").WithMap(City.Map).WithSpeed(2f);
//             EntityObject = EntityBuilder.Build();
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//             foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
//             {
//                 Object.DestroyImmediate(gameObject);
//             }
//
//             DestinationReachedFlag = false;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldFindPath()
//         {
//             Pathfinding.DestinationReached += OnDestinationReached;
//             Pathfinding.SetDestination(new Vector3Int(2, -2, 0));
//
//             yield return Pathfinding.WaitForDestinationReached;
//
//             Assert.True(DestinationReachedFlag);
//             Assert.AreEqual(new Vector3Int(2, -2, 0), Entity.CellPosition);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldFindPathFast()
//         {
//             EntityObject = EntityBuilder.WithSpeed(100f).Build();
//
//             Pathfinding.DestinationReached += OnDestinationReached;
//             Pathfinding.SetDestination(new Vector3Int(2, -2, 0));
//
//             yield return Pathfinding.WaitForDestinationReached;
//
//             Assert.True(DestinationReachedFlag);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldFindPathWithHighTimeSpeed()
//         {
//             CityBuilder = CityBuilder.WithNormalTimeSpeed(500);
//             yield return GameBuilder.Build();
//
//             EntityObject = EntityBuilder.WithMap(City.Map).Build();
//
//             Pathfinding.DestinationReached += OnDestinationReached;
//             Pathfinding.SetDestination(new Vector3Int(2, -2, 0));
//
//             yield return Pathfinding.WaitForDestinationReached;
//
//             Assert.True(DestinationReachedFlag);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldFindPathFastWithHighTimeSpeed()
//         {
//             CityBuilder = CityBuilder.WithNormalTimeSpeed(500);
//             yield return GameBuilder.Build();
//
//             EntityObject = EntityBuilder.WithMap(City.Map).WithSpeed(200f).Build();
//
//             Pathfinding.DestinationReached += OnDestinationReached;
//             Pathfinding.SetDestination(new Vector3Int(2, -2, 0));
//
//             yield return Pathfinding.WaitForDestinationReached;
//
//             Assert.True(DestinationReachedFlag);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldNotFindPathWithoutMap()
//         {
//             LogAssert.ignoreFailingMessages = true;
//
//             yield return GameBuilder.Build();
//
//             EntityObject = EntityBuilder.WithMap(City.Map).Build();
//
//             var startPosition = Entity.Position;
//
//             Object.DestroyImmediate(City.gameObject);
//
//             Pathfinding.DestinationChanged += OnDestinationChanged;
//             Pathfinding.DestinationReached += OnDestinationReached;
//
//             Pathfinding.SetDestination(new Vector3Int(3, 2, 0));
//
//             Assert.False(DestinationChangedFlag);
//             Assert.False(DestinationReachedFlag);
//             Assert.AreEqual(startPosition, Entity.Position);
//         }
//
//         void OnDestinationReached(object sender, CellEventArgs e)
//         {
//             DestinationReachedFlag = true;
//         }
//
//         void OnDestinationChanged(object sender, CellEventArgs e)
//         {
//             DestinationChangedFlag = true;
//         }
//     }
// }