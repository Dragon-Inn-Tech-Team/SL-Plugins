using CommandSystem;
using Mirror;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Commands.Spawn
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SpawnParent : ParentCommand
	{
		public override string Command => "spawn";

		public override string[] Aliases => [];

		public override string Description => "Spawn various items";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Grenade());
			RegisterCommand(new Flashbang());
			RegisterCommand(new Ball());
		}

		public static SpawnParent Create()
		{
			var cleanupCommand = new SpawnParent();
			cleanupCommand.LoadGeneratedCommands();
			return cleanupCommand;
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pCS)
			{

				var lookup = string.Join(" ", arguments).Replace(Command, string.Empty).Trim();
				LabApi.Features.Console.Logger.Info($"lookup: {lookup}");

				var prefab = NetworkClient.prefabs.Where(p => string.Equals(p.Value.name, lookup, StringComparison.InvariantCultureIgnoreCase));

				if (prefab.Any())
				{
					LabApi.Features.Console.Logger.Info($"FOUND");

					var lightGO = GameObject.Instantiate(prefab.First().Value);
					lightGO.transform.position = pCS.ReferenceHub.GetPosition();
					NetworkServer.Spawn(lightGO);

					response = $"Spawned {lookup}";
					return true;
				}
				response = $"Please provide a valid subcommand or Prefab name\n{string.Join("/", this.Commands.Select(r => r.Value.Command))}";
				return false;
			}
			else
			{
				response = "You must be a player to run this command";
				return false;
			}
		}
	}
}
