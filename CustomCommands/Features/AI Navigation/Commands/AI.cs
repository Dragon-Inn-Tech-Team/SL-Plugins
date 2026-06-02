using CommandSystem;
using CustomCommands.Features.Testing.Navigation;
using LabApi.Features.Console;
using MapGeneration;
using RedRightHand;
using RedRightHand.Commands;
using RedRightHand.Navigation.NavMeshComponents;
using RemoteAdmin;
using System;
using UnityEngine.AI;

namespace CustomCommands.Features.TestingFeatures.Commands
{
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

			if (!CustomCommandsPlugin.Config.EnableDebugTests)
			{
				response = "This command is disabled";
				return false;
			}

			if (sender is PlayerCommandSender pSender)
			{
				if (pSender.ReferenceHub.TryGetCurrentRoom(out var room))
				{
					var e = room.gameObject.TryGetComponent<NavMeshSurface>(out var nvs);

					Logger.Debug($"{e}");
				}

				foreach (var dummyHub in ReferenceHub.AllHubs)
				{
					if (dummyHub.IsDummy)
					{
						TestingDummies.AddNavMeshAgent(dummyHub.gameObject);

						if (!dummyHub.gameObject.TryGetComponent<DummyAI>(out var _))
							dummyHub.gameObject.AddComponent<DummyAI>().Init(dummyHub, dummyHub.gameObject.GetComponent<NavMeshAgent>());


						dummyHub.gameObject.TryGetComponent<DummyAI>(out var ai);

						NavigationManager.SetDestination(pSender.ReferenceHub.transform.position, dummyHub.gameObject.GetComponent<NavMeshAgent>());

						response = $"Path set";
					}
				}
			}

			//response = $"Path set to";
			return true;
		}
	}
}
