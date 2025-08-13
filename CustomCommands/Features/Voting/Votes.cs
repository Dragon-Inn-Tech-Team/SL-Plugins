

using CustomCommands.Core;
using Discord;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins.Enums;
using System.Collections.Generic;

namespace CustomCommands.Features.Voting
{
	public class Votes : CustomFeature
	{
		public Votes(bool configSetting) : base(configSetting)
		{
		}

		public static bool VoteInProgress => CurrentVote != VoteType.NONE;
		public static VoteType CurrentVote = VoteType.NONE;
		public static string CurrentVoteString = string.Empty;

		public static Dictionary<string, bool> PlayerVotes = new Dictionary<string, bool>();

		public static void StartVote(VoteType type, string vStr)
		{
			SetVote(type, vStr);

			foreach (var a in Player.GetAll())
			{
				a.SendBroadcast($"<b><color=#fa886b>[VOTE]</color></b> <color=#79b7ed>{vStr}</color>\nUse your console (Press ' to open) to vote now!", 15);
				a.SendConsoleMessage("A vote has been started. Run the `.yes` command to vote yes, or `.no` command to vote no");
			}

			MEC.Timing.CallDelayed(3 * 60, () =>
			{
				Votes.EndVote();

			});
		}

		public static void SetVote(VoteType type, string vStr)
		{
			CurrentVote = type;
			CurrentVoteString = vStr;
		}

		public static void EndVote()
		{
			if (!VoteInProgress)
				return;

			int yes = 0;
			int no = 0;
			int nil = 0;

			foreach (var a in PlayerVotes)
			{
				if (a.Value)
				{
					yes++;
				}
				else if (!a.Value)
				{
					no++;
				}
				else
					nil++;
			}

			PlayerVotes.Clear();
			Server.SendBroadcast($"The vote is over!\n<color=green>{yes} voted yes</color>, <color=red>{no} voted no</color>, and {nil} did not vote", 10);
			SetVote(VoteType.NONE, string.Empty);
		}

		public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
		{
			if (VoteInProgress)
			{
				ev.Player.SendBroadcast($"<b><color=#fa886b>[VOTE]</color></b> <color=#79b7ed>{CurrentVoteString}</color>\nUse your console to vote now!", 15);
				ev.Player.SendConsoleMessage("A vote has been started. Run the `.yes` command to vote yes, or `.no` command to vote no");
			}
		}

		public override void OnServerRoundStarted()
		{
			SetVote(VoteType.NONE, string.Empty);
		}

		public override void OnServerRoundEnded(RoundEndedEventArgs ev)
		{
			EndVote();
		}
	}
}
