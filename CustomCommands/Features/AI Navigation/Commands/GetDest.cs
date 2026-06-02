using CommandSystem;
using LabApi.Features.Console;
using RedRightHand;
using RedRightHand.Commands;
using RemoteAdmin;
using System;
using UnityEngine.AI;

namespace CustomCommands.Features.TestingFeatures.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GetDest : ICustomCommand
	{
		public string Command => "dummydest";

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

			if (sender is PlayerCommandSender pSender)
			{
				foreach (var dummyHub in ReferenceHub.AllHubs)
				{
					if (dummyHub.IsDummy)
					{
						if (dummyHub.gameObject.TryGetComponent<NavMeshAgent>(out var agent))
						{
							Logger.Info($"{agent.destination} {agent.remainingDistance} {agent.pathStatus} {agent.path.status} {agent.isOnNavMesh}");
						}
					}
				}
			}

			response = $"Dist Printed to server console";

			return true;
		}
	}
}
