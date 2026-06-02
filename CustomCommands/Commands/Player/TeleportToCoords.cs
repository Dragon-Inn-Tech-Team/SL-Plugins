using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;
using UnityEngine;

namespace CustomCommands.Commands.Player
{
	[CommandHandler(typeof(PlayerParent))]
	public class TeleportToCoords : ICustomCommand
	{
		public string Command => "tptocoords";

		public string[] Aliases { get; } = { "tpc" };

		public string Description => "Teleports a player to the specified co-ordinates";

		public string[] Usage { get; } = { "%player%", "x", "y", "z" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.teleporting";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			if (!float.TryParse(arguments.Array[2], out float x) || !float.TryParse(arguments.Array[3], out float y) || !float.TryParse(arguments.Array[4], out float z))
			{
				response = "Valid coords not provided";
				return false;
			}

			Vector3 pos = new Vector3(x, y, z);

			foreach (var plr in players)
			{
				if (plr.Role == PlayerRoles.RoleTypeId.Spectator)
					plr.SetRole(PlayerRoles.RoleTypeId.Tutorial, PlayerRoles.RoleChangeReason.RemoteAdmin);

				plr.Position = pos;
			}

			response = $"Teleported {players.Count} {(players.Count == 1 ? "player" : "players")} to position ({x}, {y}, {z})";
			return true;
		}
	}
}
