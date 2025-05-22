using CommandSystem;
using RedRightHand.Core.Commands;
using System;
using System.Linq;
using VoiceChat;

namespace CustomCommands.Commands.Misc
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class MuteCheckCommand : ICustomCommand
	{
		public string Command => "mutecheck";

		public string[] Aliases { get; } = { "mcheck", "mutec" };

		public string Description => "Checks if a specific UserID is muted";

		public string[] Usage { get; } = { "UserID" };

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 1)
			{
				response = "Invalid UserID provided";
				return false;
			}

			var searchTerm = arguments.First();
			VoiceChatMutes.QueryLocalMute(searchTerm);

			if (VoiceChatMutes.QueryLocalMute(searchTerm))
			{
				response = $"User {searchTerm} is muted";
			}
			else
			{
				response = $"User {searchTerm} is not muted";
			}

			return true;
		}
	}
}
