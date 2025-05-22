using PluginAPI.Core;

namespace CustomCommands.Features.Voting
{
	public static class VoteManager
	{
		public static bool VoteInProgress => CurrentVote != VoteType.NONE;
		public static VoteType CurrentVote = VoteType.NONE;
		public static string CurrentVoteString = string.Empty;

		public static void SetVote(VoteType type, string vStr)
		{
			VoteManager.CurrentVote = type;
			VoteManager.CurrentVoteString = vStr;
		}

		public static void EndVote()
		{
			if (!VoteManager.VoteInProgress)
				return;

			int yes = 0;
			int no = 0;
			int nil = 0;

			foreach (var a in Player.GetPlayers())
			{
				if (a.TemporaryData.Contains("vote_yes"))
				{
					yes++;
					a.TemporaryData.Remove("vote_yes");
				}
				else if (a.TemporaryData.Contains("vote_no"))
				{
					no++;
					a.TemporaryData.Remove("vote_no");
				}
				else
					nil++;
			}

			Server.SendBroadcast($"The vote is over!\n<color=green>{yes} voted yes</color>, <color=red>{no} voted no</color>, and {nil} did not vote", 10);
			SetVote(VoteType.NONE, string.Empty);
		}
	}
}
