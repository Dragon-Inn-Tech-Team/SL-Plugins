using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Commands.Spawn
{
	[CommandHandler(typeof(SpawnParent))]
	public class Ball : ICustomCommand
	{
		public string Command => "ball";

		public string[] Aliases => null;

		public string Description => "Spawns SCP018 at a player's location";

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

				Helpers.SpawnGrenade<InventorySystem.Items.ThrowableProjectiles.Scp018Projectile>(plr, ItemType.SCP018);
			}

			response = $"Spawned SCP-018 on {players.Count} {(players.Count > 1 ? "players" : "player")}";
			return true;
		}
	}
}
