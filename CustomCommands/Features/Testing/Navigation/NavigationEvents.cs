using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using RedRightHand.Core.Navigation.NavMeshComponents;
using UnityEngine;
using UnityEngine.AI;

namespace CustomCommands.Features.Testing.Navigation
{
	public class NavigationEvents
	{
		[PluginEvent]
		public void MapGeneratedEvent(MapGeneratedEvent ev)
		{
			foreach (var door in Facility.Doors)
			{
				if (door.Permissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
				{
					var nmo = door.GameObject.AddComponent<NavMeshObstacle>();
					nmo.carving = true;
				}
			}

			var rooms = GameObject.Find("LightRooms");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				var settings = meshSurface.GetBuildSettings();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.ignoreNavMeshObstacle = true;
				meshSurface.voxelSize = 0.08f;
				meshSurface.buildHeightMesh = true;
				settings.agentSlope = 90;
				meshSurface.BuildNavMesh();
			}

			rooms = GameObject.Find("HeavyRooms");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				var settings = meshSurface.GetBuildSettings();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.ignoreNavMeshObstacle = true;
				meshSurface.voxelSize = 0.08f;
				meshSurface.buildHeightMesh = true;
				settings.agentSlope = 90;
				meshSurface.BuildNavMesh();
			}

			rooms = GameObject.Find("EntranceRooms");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				var settings = meshSurface.GetBuildSettings();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.ignoreNavMeshObstacle = true;
				meshSurface.voxelSize = 0.08f;
				meshSurface.buildHeightMesh = true;
				settings.agentSlope = 90;
				meshSurface.BuildNavMesh();
			}

			rooms = GameObject.Find("Outside");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				var settings = meshSurface.GetBuildSettings();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.ignoreNavMeshObstacle = true;
				meshSurface.voxelSize = 0.08f;
				meshSurface.buildHeightMesh = true;
				settings.agentSlope = 90;
				meshSurface.BuildNavMesh();
			}
		}
	}
}
