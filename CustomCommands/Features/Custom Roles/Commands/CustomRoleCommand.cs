using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;
using System.Linq;

namespace CustomCommands.Features.Custom.Roles.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CustomRoleCommand : ICustomCommand
	{
		public PlayerPermissions? Permission => PlayerPermissions.ForceclassWithoutRestrictions;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public string Command => "customrole";

		public string[] Aliases => ["cr"];

		public string Description => "Forces a player to a custom role";

		public string[] Usage => ["%player%", "role"];

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out var pSender))
				return false;

			if (Enum.TryParse<CustomRolesManager.CustomRoleType>(arguments.ElementAt(1), true, out var roleEnum))
			{
				foreach (var player in players)
				{
					CustomRolesManager.EnableRole(player, roleEnum);
				}

				response = $"{players.Count} player(s) set to custom role {arguments.ElementAt(1)}";
				return true;
			}
			else
			{
				response = $"Unable to find role {arguments.ElementAt(1)}. It may not exist, or hasn't been enabled";
				return false;
			}
		}
	}
}
