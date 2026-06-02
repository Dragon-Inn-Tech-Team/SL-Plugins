using CustomCommands.Core;
using CustomCommands.Features.Testing.Navigation;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using NetworkManagerUtils.Dummies;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using UnityEngine.AI;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.TestingFeatures
{
	public class TestingDummies : CustomFeatureBase
	{
		public readonly static NavMeshBuildSettings navMeshBuildSettings = new NavMeshBuildSettings()
		{
			agentRadius = 0.1f,
			agentHeight = 2f,
			agentClimb = 2f,
			agentSlope = 60f,
			voxelSize = 0.1f
		};

		public static void AddNavMeshAgent(GameObject go)
		{
			if (!go.TryGetComponent<NavMeshAgent>(out var agent))
			{
				agent = go.AddComponent<NavMeshAgent>();

				agent.baseOffset = 0.4f;
				agent.updateRotation = true;
				agent.angularSpeed = 360;
				agent.acceleration = 600;
				agent.radius = navMeshBuildSettings.agentRadius;
				agent.areaMask = 1;
				agent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
			}
		}

		public static DummyAI CreateNewSmartDummy(ReferenceHub hub)
		{
			AddNavMeshAgent(hub.gameObject);

			if (!hub.gameObject.TryGetComponent<DummyAI>(out var ai))
				hub.gameObject.AddComponent<DummyAI>().Init(hub, hub.gameObject.GetComponent<NavMeshAgent>());

			return hub.gameObject.GetComponent<DummyAI>();
		}

		public TestingDummies(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerWaitingForPlayers()
		{
			if (CustomCommandsPlugin.Config.EnableSteve)
			{
				var steve = DummyUtils.SpawnDummy("Steve");
				Round.IsLocked = true;
			}
		}

		public override void OnServerRoundStarted()
		{
			Timing.CallDelayed(0.2f, () =>
			{
				foreach (var dummyHub in ReferenceHub.AllHubs)
				{
					if (dummyHub.IsDummy)
					{
						var dummyAI = CreateNewSmartDummy(dummyHub);
						NavigationManager.SetDestination(dummyHub.GetPosition(), dummyHub.gameObject.GetComponent<NavMeshAgent>());
					}
				}
			});
		}

		public override void OnPlayerHurt(PlayerHurtEventArgs ev)
		{
			Logger.Debug("BLEH");

			if (ev.Player.IsDummy)
			{
				Logger.Debug($"BLEH 2 {ev.Player.ReferenceHub.gameObject.TryGetComponent<DummyAI>(out _)}");

				if (ev.Player.ReferenceHub.gameObject.TryGetComponent<DummyAI>(out var agnet))
				{
					Logger.Debug($"BLEH 3");
					NavigationManager.SetDestination(ev.Attacker.Position, agnet.gameObject.GetComponent<NavMeshAgent>());
				}
			}
		}
	}
}
