using CommandSystem;
using RedRightHand.Core.Commands;
using System;
using System.Linq;
using VoiceChat;

namespace CustomCommands.Commands.Misc
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RemoteMuteCommand : ICustomCommand
	{
		public string Command => "rmute";

		public string[] Aliases { get; } = { "remotemute" };

		public string Description => "Mutes a user remotely";

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
				VoiceChatMutes.RevokeLocalMute(searchTerm);
				response = $"User {searchTerm} has been unmuted";
			}
			else
			{
				VoiceChatMutes.IssueLocalMute(searchTerm);
				response = $"User {searchTerm} has been muted";
			}

			return true;
		}
	}
}
