﻿using CommandSystem;
using CustomCommands.Commands.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceChat;

namespace CustomCommands.Commands.Misc
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class RemoteMuteCommand : ICommand
	{
		public string Command => "rmute";

		public string[] Aliases { get; } = ["remotemute"];

		public string Description => "Mutes a user remotely";

		public string[] Usage { get; } = ["UserID"];

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
