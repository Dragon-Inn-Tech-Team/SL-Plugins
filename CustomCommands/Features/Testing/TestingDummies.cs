using CommandSystem;
using GameCore;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using RemoteAdmin;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace CustomCommands.Features.Testing
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Fill : ICustomCommand
	{
		public string Command => "dummyfill";

		public string[] Aliases => null;

		public string Description => "Fills the server with dummy players";

		public string[] Usage { get; } = { "name" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyf";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (!Plugin.Config.EnableDebugTests)
			{
				response = "This command is disabled";
				return false;
			}

			var dumsToMake = Server.MaxPlayers - Server.PlayerCount;

			for (int i = 0; i < dumsToMake; i++)
			{
				DummyUtils.SpawnDummy(arguments.ElementAt(0) + i);
			}

			response = $"Server filled with {dumsToMake} dummies";

			return true;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class AI : ICustomCommand
	{
		public string Command => "dummyai";

		public string[] Aliases => null;

		public string Description => "Basic AI for dummies";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyf";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (!Plugin.Config.EnableDebugTests)
			{
				response = "This command is disabled";
				return false;
			}

			if (sender is PlayerCommandSender pSender)
			{
				foreach (var dummyHub in ReferenceHub.AllHubs)
				{
					if (dummyHub.IsDummy)
					{
						if (!dummyHub.gameObject.TryGetComponent<NavMeshAgent>(out var agent))
						{
							agent = dummyHub.gameObject.AddComponent<NavMeshAgent>();

							agent.baseOffset = 0.98f;
							agent.updateRotation = true;
							agent.angularSpeed = 360;
							agent.acceleration = 30;
							agent.height = 1.3f;
							agent.speed = 5f;
							agent.baseOffset = 0.5f;
							agent.stoppingDistance = 1f;

							agent.radius = 0.5f;
							agent.areaMask = 1;
							agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
						}

						if (!dummyHub.gameObject.TryGetComponent<DummyAI>(out var ai))
							dummyHub.gameObject.AddComponent<DummyAI>().Init(dummyHub, agent);

						agent.SetDestination(pSender.ReferenceHub.transform.position);

						foreach (var corner in agent.path.corners)
						{
							PluginAPI.Core.Log.Info($"{corner} + {pSender.ReferenceHub.transform.position}");
						}

						response = $"Path set with {agent.path.corners.Length} corners";
					}
				}
			}

			//response = $"Path set to";
			return true;
		}
	}

	public class DummyAI : MonoBehaviour
	{
		private ReferenceHub _hub;
		private NavMeshAgent _agent;
		private float _speed;
		private int _index;

		public void Init(ReferenceHub hub, NavMeshAgent _agent, float speed = 30f)
		{
			_hub = hub;
			this._agent = _agent;
			_speed = speed;
			_index = 0;
		}

		private void Update()
		{
			if (NetworkServer.active)
			{
				IFpcRole fpcRole = _hub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					FirstPersonMovementModule fpcModule = fpcRole.FpcModule;
					Vector3 pos = _hub.transform.position;
					var dist = Vector3.Distance(pos, _agent.destination);
					if (dist > _agent.stoppingDistance)
					{
						fpcModule.MouseLook.LookAtDirection(fpcModule.Motor.Velocity);
					}

					return;
				}
			}

			Destroy(this);
		}
	}

	public class TestingDummies
	{
		[PluginEvent]
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			DummyUtils.SpawnDummy("Steve");
			Round.IsLocked = true;
		}
	}
}
