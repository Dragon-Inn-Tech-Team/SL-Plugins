
using CustomCommands.Core;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using Interactables.Interobjects.DoorUtils;
using RedRightHand.Navigation.NavMeshComponents;
using UnityEngine;
using UnityEngine.AI;
using Logger = LabApi.Features.Console.Logger;
using UnityEngine.SceneManagement;

namespace CustomCommands.Features.Testing.Navigation
{
	public class NavigationEvents : CustomFeature
	{
		public NavigationEvents(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerMapGenerated(MapGeneratedEventArgs ev)
		{
			foreach (var door in Map.Doors)
			{
				if (door.Permissions == DoorPermissionFlags.None)
				{
					var nmo = door.GameObject.AddComponent<NavMeshObstacle>();
					nmo.carving = false;
				}
			}

			//foreach (var a in SceneManager.GetActiveScene().GetRootGameObjects())
			//{
			//	Logger.Info(a.gameObject.name);
			//}

			var root = GameObject.Find("MapgenRoot");
			if(root != null)
			{
				var meshSurface = root.AddComponent<NavMeshSurface>();
				var settings = meshSurface.GetBuildSettings();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.layerMask = new LayerMask() { value = 305624887 };
				meshSurface.voxelSize = 0.08f;
				meshSurface.BuildNavMesh();

				foreach (Transform a in root.transform)
				{
					Logger.Info(a.gameObject.name);
				}
			}


			//var rooms = GameObject.Find("LightRooms");
			//if (rooms != null)
			//{
			//	var meshSurface = rooms.AddComponent<NavMeshSurface>();
			//	var settings = meshSurface.GetBuildSettings();
			//	meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
			//	meshSurface.layerMask = new LayerMask() { value = 305624887 };
			//	meshSurface.voxelSize = 0.08f;
			//	meshSurface.BuildNavMesh();
			//}

			//rooms = GameObject.Find("HeavyRooms");
			//if (rooms != null)
			//{
			//	var meshSurface = rooms.AddComponent<NavMeshSurface>();
			//	var settings = meshSurface.GetBuildSettings();
			//	meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
			//	meshSurface.layerMask = new LayerMask() { value = 305624887 };
			//	meshSurface.voxelSize = 0.08f;
			//	meshSurface.BuildNavMesh();
			//}

			//rooms = GameObject.Find("EntranceRooms");
			//if (rooms != null)
			//{
			//	var meshSurface = rooms.AddComponent<NavMeshSurface>();
			//	var settings = meshSurface.GetBuildSettings();
			//	meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
			//	meshSurface.layerMask = new LayerMask() { value = 305624887 };
			//	meshSurface.voxelSize = 0.08f;
			//	meshSurface.BuildNavMesh();
			//}

			//rooms = GameObject.Find("Outside");
			//if (rooms != null)
			//{
			//	var meshSurface = rooms.AddComponent<NavMeshSurface>();
			//	var settings = meshSurface.GetBuildSettings();
			//	meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
			//	meshSurface.layerMask = new LayerMask() { value = 305624887 };
			//	meshSurface.voxelSize = 0.08f;
			//	meshSurface.BuildNavMesh();
			//}
		}
	}
}
