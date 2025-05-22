using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace CustomCommands.Features.Voting
{
	internal class VotingEvents
	{
		[PluginEvent, PluginPriority(LoadPriority.Lowest)]
		public void PlayerJoin(PlayerJoinedEvent args)
		{
			if (VoteManager.VoteInProgress)
			{
				args.Player.SendBroadcast($"<b><color=#fa886b>[VOTE]</color></b> <color=#79b7ed>{VoteManager.CurrentVoteString}</color>\nUse your console to vote now!", 15);
				args.Player.SendConsoleMessage("A vote has been started. Run the `.yes` command to vote yes, or `.no` command to vote no");
			}
		}

		[PluginEvent]
		public void RoundRestart(RoundRestartEvent args)
		{
			VoteManager.SetVote(VoteType.NONE, string.Empty);
		}

		[PluginEvent]
		public void RoundEnd(RoundEndEvent args)
		{
			VoteManager.EndVote();
		}


	}
}
