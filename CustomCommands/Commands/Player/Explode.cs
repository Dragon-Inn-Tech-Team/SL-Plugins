using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;
using Utils;

namespace CustomCommands.Commands.Player
{
	[CommandHandler(typeof(PlayerParent))]
	public class Explode : ICustomCommand
	{
		public string Command => "explode";

		public string[] Aliases => null;

		public string Description => "Causes the player to explode";

		public string[] Usage { get; } = ["%player%"];

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
				ExplosionUtils.ServerExplode(plr.ReferenceHub, ExplosionType.PinkCandy);
			}
			response = "Player successfully detonated";
			return true;
		}
	}
}
