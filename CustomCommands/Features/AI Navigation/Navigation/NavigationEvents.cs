
using CustomCommands.Core;
using CustomCommands.Features.TestingFeatures;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MapGeneration.RoomConnectors;
using MEC;
using RedRightHand.Navigation.NavMeshComponents;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.Testing.Navigation
{
	public class NavigationEvents : CustomFeatureBase
	{
		public static Dictionary<string, NavMeshSurface> NavMeshCache = new Dictionary<string, NavMeshSurface>();
		public NavigationEvents(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerMapGenerated(MapGeneratedEventArgs ev)
		{
			Timing.CallDelayed(0, () =>
			{
				MEC.Timing.RunCoroutine(generateNavMesh());
			});
		}
		public override void OnPlayerInteractedDoor(PlayerInteractedDoorEventArgs ev)
		{
			foreach (var room in ev.Door.Rooms)
			{
				room.GameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
			}
		}

		public IEnumerator<float> generateNavMesh()
		{
			Logger.Info($"Generating Meshes");

			foreach (var door in Map.Doors)
			{
				var nmo = door.GameObject.AddComponent<NavMeshModifier>();
				nmo.ignoreFromBuild = true;
			}

			foreach (var connector in RoomConnectorDistributorSettings.RegisteredConnectors)
			{
				var nmo = connector.gameObject.AddComponent<NavMeshModifier>();
				nmo.ignoreFromBuild = true;
			}

			foreach (var a in Map.Rooms)
			{
				MEC.Timing.RunCoroutine(buildNavMesh(a.GameObject));
			}

			Logger.Info($"Mesh Generation Complete!");

			NavMeshCache.Clear();
			Logger.Info($"NavMesh Cache Cleared!");

			yield return 0f;
		}
		public static IEnumerator<float> buildNavMesh(GameObject go)
		{
			var meshSurface = go.AddComponent<NavMeshSurface>();
			var settings = meshSurface.GetBuildSettings();
			settings = TestingDummies.navMeshBuildSettings;
			meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

			if (NavMeshCache.ContainsKey(go.name))
			{
				meshSurface.navMeshData = NavMeshCache[go.name].navMeshData;
				Logger.Info($"{go.name} NavMesh Copied");
			}
			else
			{
				meshSurface.BuildNavMesh();
				Logger.Info($"{go.name} NavMesh built");
				NavMeshCache.Add(go.name, meshSurface);
			}

			yield return 0f;
		}
	}
}
