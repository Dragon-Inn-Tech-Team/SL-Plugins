using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Features.Items.Grenades.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class BallCommand : ICustomCommand
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

			foreach (Player plr in players)
			{
				if (plr.Role == PlayerRoles.RoleTypeId.Spectator || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				ItemManager.SpawnGrenade<Scp018Projectile>(plr, ItemType.SCP018);
			}

			response = $"Spawned SCO-018 on {players.Count} {(players.Count > 1 ? "players" : "player")}";
			return true;
		}
	}
}
