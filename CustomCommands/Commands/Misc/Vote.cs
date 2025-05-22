using CommandSystem;
using CustomCommands.Features.Voting;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Commands.Misc
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Vote : ICustomCommand
	{
		public string Command => "vote";

		public string[] Aliases => null;

		public string Description => "Triggers a vote for players";

		public string[] Usage { get; } = { "message" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.playervote";

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out var pSender))
				return false;

			if (VoteManager.VoteInProgress)
			{
				response = "There is already a vote in progress";
				return false;
			}

			var msg = string.Join(" ", arguments).Replace(Command, string.Empty).Trim();
			VoteManager.SetVote(VoteType.AdminVote, msg);

			foreach (var a in Player.GetPlayers())
			{
				a.SendBroadcast($"<b><color=#fa886b>[VOTE]</color></b> <color=#79b7ed>{msg}</color>\nUse your console (Press ' to open) to vote now!", 15);
				a.SendConsoleMessage("A vote has been started. Run the `.yes` command to vote yes, or `.no` command to vote no");
			}

			MEC.Timing.CallDelayed(3 * 60, () =>
			{
				VoteManager.EndVote();

			});
			response = "Vote has been started";
			return true;
		}
	}
}
