using CommandSystem;
using PluginAPI.Core;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.Voting.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Yes : ICommand
	{
		public string Command => "yes";

		public string[] Aliases => null;
		public string Description => "Vote yes on the current vote";

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender)
			{
				if (!VoteManager.VoteInProgress)
				{
					response = "There is no vote in progress";
					return false;
				}

				var plr = Player.Get(pSender.ReferenceHub);

				if (plr.TemporaryData.Contains("vote_yes") || plr.TemporaryData.Contains("vote_no"))
				{
					response = "You have already voted";
					return false;
				}

				plr.TemporaryData.Override("vote_yes", string.Empty);

				response = "You have voted yes";
				return false;
			}

			response = "You must be a player to run this command";
			return false;
		}
	}
}
