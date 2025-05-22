using CommandSystem;
using RedRightHand.Core.Commands;
using System;
using System.Linq;

namespace CustomCommands.Commands.Misc
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class BanCheckCommand : ICustomCommand
	{
		public string Command => "bancheck";

		public string[] Aliases { get; } = { "bcheck", "banc" };

		public string Description => "Checks if a specific UserID or IP address is banned";

		public string[] Usage { get; } = { "UserID/IP" };

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 1)
			{
				response = "Invalid UserID/IP provided";
				return false;
			}

			var searchTerm = arguments.First();

			BanDetails details;

			if (!searchTerm.Contains('@'))
			{
				var kVP = BanHandler.QueryBan(string.Empty, searchTerm);
				details = kVP.Value;
			}
			else
			{
				var kVP = BanHandler.QueryBan(searchTerm, string.Empty);
				details = kVP.Key;
			}

			if (details != null)
			{
				response = $"User {searchTerm} is banned. Banned until: {new DateTime(details.Expires):dd/MM/yyyy HH:mm}";
			}
			else
			{
				response = $"User {searchTerm} is not banned";
			}

			return true;
		}
	}
}
