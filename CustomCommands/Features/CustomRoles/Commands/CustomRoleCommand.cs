using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using RedRightHand.CustomRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomRoles.Commands
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

			var role = arguments.ElementAt(1);

			if (CustomRolesManager.AvailableRoles.ContainsKey(role))
			{
				foreach(var player in players)
				{
					CustomRolesManager.EnableRole(player, role);
				}

				response = $"{players.Count} player(s) set to custom role {role}";
				return true;
			}
			else
			{
				response = $"Unable to find role {role}. It may not exist, or hasn't been enabled";
				return false;
			}
		}
	}
}
