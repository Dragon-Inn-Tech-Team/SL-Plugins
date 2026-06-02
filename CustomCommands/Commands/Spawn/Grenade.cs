using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Commands.Spawn
{
	[CommandHandler(typeof(SpawnParent))]
	public class Grenade : ICustomCommand
	{
		public string Command => "grenade";

		public string[] Aliases => null;

		public string Description => "Spawns a grenade at a player's location";

		public string[] Usage { get; } = { "%player%" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.grenade";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			foreach (var plr in players)
			{
				if (plr.Role == PlayerRoles.RoleTypeId.Spectator || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				Helpers.SpawnGrenade<TimeGrenade>(plr, ItemType.GrenadeHE);
			}

			response = $"Spawned a grenade on {players.Count} {(players.Count > 1 ? "players" : "player")}";
			return true;
		}
	}
}
