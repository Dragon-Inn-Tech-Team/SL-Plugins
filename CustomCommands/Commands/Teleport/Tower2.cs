using CommandSystem;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using UnityEngine;

namespace CustomCommands.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Tower2 : ICustomCommand
	{
		public string Command => "tower2";

		public string[] Aliases => null;

		public string Description => "Teleports the player to a second tower on the surface";

		public string[] Usage { get; } = { "%player%" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.teleporting";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			foreach (Player plr in players)
			{
				if (plr.Role == PlayerRoles.RoleTypeId.Spectator)
					plr.SetRole(PlayerRoles.RoleTypeId.Tutorial, PlayerRoles.RoleChangeReason.RemoteAdmin);

				plr.Position = new Vector3(-15.5f, 1014.5f, -31.5f);
			}

			response = $"Teleported {players.Count} {(players.Count == 1 ? "player" : "players")} to tower 2";
			return true;
		}
	}
}
