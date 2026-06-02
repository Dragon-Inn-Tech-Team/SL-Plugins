using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.Voting.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class StartVote : ICustomCommand
	{
		public string Command => "startvote";

		string[] ICommand.Aliases { get; } = ["sv"];
		public string Description => "Start a server wide vote";

		public bool SanitizeResponse => false;

		public PlayerPermissions? Permission => PlayerPermissions.GivingItems;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public string[] Usage => ["Text"];

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (Votes.VoteInProgress)
			{
				response = "There is already a vote in progress";
				return false;
			}

			var msg = string.Join(" ", arguments).Replace(Command, string.Empty).Trim();
			Votes.StartVote(VoteType.AdminVote, msg);

			response = "Vote has been started";
			return true;
		}
	}
}
