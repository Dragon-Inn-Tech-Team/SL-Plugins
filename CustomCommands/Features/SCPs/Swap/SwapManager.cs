using PlayerRoles;
using PlayerRoles.RoleAssign;
using PluginAPI.Core;
using RedRightHand.Core;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.SCPs.Swap
{
	public static class SwapManager
	{
		private static int _swapSeconds = 60;
		public static int SwapToHumanSeconds => _swapSeconds;
		public static int SwapToScpSeconds => (int)(_swapSeconds * 1.5f);

		public static int SCPsToReplace = 0;
		public static int ReplaceBaseCooldownRounds = 3;
		public static Dictionary<string, int> triggers = new Dictionary<string, int>();
		public static Dictionary<string, int> scpCooldown = new Dictionary<string, int>();
		public static Dictionary<string, int> humanCooldown = new Dictionary<string, int>();
		public static Dictionary<ReferenceHub, uint> raffleParticipants = new Dictionary<ReferenceHub, uint>();
		private static List<RoleTypeId> scpRoles = new List<RoleTypeId>() { RoleTypeId.Scp049, RoleTypeId.Scp079, RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939, RoleTypeId.Scp096 };

		public static void ReplaceBroadcast()
		{
			Server.ClearBroadcasts();
			Server.SendBroadcast($"There {(SCPsToReplace == 1 ? "is" : "are")} now {SCPsToReplace} SCP spot{(SCPsToReplace == 1 ? "" : "s")} available. Run \".scp\" to queue for an SCP", 5);
		}

		public static RoleTypeId[] AvailableSCPs
		{
			get
			{
                var tempRoles = scpRoles;
                var currentScpRoles = Player.GetPlayers().Where(r => r.ReferenceHub.IsSCP()).Select(r => r.Role);
				
				foreach (var r in currentScpRoles)
				{
					if (tempRoles.Contains(r))
                        tempRoles.Remove(r);

                    if (r == RoleTypeId.Scp079 || r == RoleTypeId.Scp096) // 079/096 exclusivity
                    {
                        tempRoles.Remove(RoleTypeId.Scp079);
                        tempRoles.Remove(RoleTypeId.Scp096);
                    }
                }

                if (tempRoles.Any()) // Incase this is used on a really big server
                    return tempRoles.ToArray();
                else
                    return scpRoles.ToArray();
            }
		}

		public static bool CanScpSwapToHuman(ReferenceHub plr, out string reason) => CanScpSwapToHuman(Player.Get(plr), out reason);
		public static bool CanScpSwapToHuman(Player plr, out string reason)
		{
			if (!plr.IsSCP || plr.Role == RoleTypeId.Scp0492)
			{
				reason = "You must be an SCP to run this command";
				return false;
			}

			if (plr.Health != plr.MaxHealth)
			{
				reason = "You cannot swap as you have taken damage";
				return false;
			}
			if (plr.TemporaryData.Contains("replacedscp"))
			{
				reason = "You cannot swap back to human";
				return false;
			}
			if (humanCooldown.TryGetValue(plr.UserId, out int roundCount))
			{
				if (roundCount > RoundRestart.UptimeRounds)
				{
					reason = $"You are on cooldown for another {roundCount - RoundRestart.UptimeRounds} round(s).";
					return false;
				}
			}
			if (Round.Duration > TimeSpan.FromSeconds(SwapToHumanSeconds))
			{
				reason = $"You can only swap from SCP within the first {SwapToHumanSeconds} seconds of a round";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		public static bool CanHumanSwapToScp(ReferenceHub plr, out string reason) => CanHumanSwapToScp(Player.Get(plr), out reason);
		public static bool CanHumanSwapToScp(Player plr, out string reason)
		{
			if (SCPsToReplace < 1)
			{
				reason = "There are no SCPs to replace";
				return false;
			}
			if (plr.TemporaryData.Contains("startedasscp"))
			{
				reason = "You were already an SCP this round";
				return false;
			}
			if (Round.Duration > TimeSpan.FromSeconds(SwapToScpSeconds))
			{
				reason = $"You can only replace an SCP within the first {SwapToScpSeconds} seconds of the round";
				return false;
			}
			if (scpCooldown.TryGetValue(plr.UserId, out int roundCount))
			{
				if (roundCount > RoundRestart.UptimeRounds)
				{
					if (SwapManager.triggers.TryGetValue(plr.UserId, out int count))
					{
						if (count > 2)
						{
							SwapManager.scpCooldown[plr.UserId]++;
							SwapManager.triggers[plr.UserId] = 0;
						}
						else SwapManager.triggers[plr.UserId]++;
					}
					else
						SwapManager.triggers.Add(plr.UserId, 1);


					reason = $"You are on cooldown for another {SwapManager.scpCooldown[plr.UserId] - RoundRestart.UptimeRounds} round(s).";
					return false;
				}
			}

			reason = string.Empty;
			return true;
		}

		public static void SwapScpToHuman(ReferenceHub plr) => SwapScpToHuman(Player.Get(plr));
		public static void SwapScpToHuman(Player plr)
		{
			SwapManager.SCPsToReplace++;
			HumanSpawner.SpawnLate(plr.ReferenceHub);
			plr.TemporaryData.Add("startedasscp", true.ToString());
			SwapManager.ReplaceBroadcast();

			humanCooldown.AddToOrReplaceValue(plr.UserId, RoundRestart.UptimeRounds + SwapManager.ReplaceBaseCooldownRounds);
		}

		public static void QueueSwapHumanToScp(ReferenceHub plr) => QueueSwapHumanToScp(Player.Get(plr));
		public static void QueueSwapHumanToScp(Player plr)
		{
            bool first = raffleParticipants.Count == 0;

            ScpTicketsLoader tix = new ScpTicketsLoader();
            int numTix = tix.GetTickets(plr.ReferenceHub, 10);
            tix.Dispose();
            uint rGroup = 1;
            if (numTix >= 13) rGroup = (uint)numTix;
            if (plr.Role == RoleTypeId.Spectator) rGroup <<= 8;

            if (!raffleParticipants.ContainsKey(plr.ReferenceHub))
                plr.ReceiveHint("You are now in the SCP raffle", 3);

            raffleParticipants.AddToOrReplaceValue(plr.ReferenceHub, rGroup);

            if (first)
            {
                MEC.Timing.CallDelayed(5f, () =>
                {
                    ReferenceHub draw;
                yoinkus: //Makes sure the person didn't leave in the 5 second draw time and that all SCP slots are filled
                    if (raffleParticipants.Count == 0)
                        return;

                    List<KeyValuePair<ReferenceHub, uint>> DrawGroup = raffleParticipants.ToList();
                    if (raffleParticipants.Count >= 6)
                    {
                        DrawGroup.Sort((x, y) => x.Value.CompareTo(y.Value));
                        DrawGroup = DrawGroup.Skip(DrawGroup.Count / 2).ToList();
                    }

                    draw = DrawGroup.PullRandomItem().Key;

                    var drawPlr = Player.Get(draw);
                    if (drawPlr != null)
                        SwapHumanToScp(drawPlr);
                    else goto yoinkus;

                    if (SCPsToReplace == 0)
                    {
                        while (raffleParticipants.Count > 0)
                        {
                            var temp = Player.Get(raffleParticipants.First().Key);
                            if (temp != null)
                                temp.ReceiveHint("You lost the raffle.", 3);
                            raffleParticipants.Remove(raffleParticipants.First().Key);
                        }
                    }
                    else goto yoinkus;
                });
            }
        }

		public static void SwapHumanToScp(ReferenceHub plr) => SwapHumanToScp(Player.Get(plr));
		public static void SwapHumanToScp(Player plr)
		{
			var scps = SwapManager.AvailableSCPs;

			plr.SetRole(scps[new Random().Next(0, scps.Length)], RoleChangeReason.LateJoin);
			ScpTicketsLoader tix = new ScpTicketsLoader();
			tix.ModifyTickets(plr.ReferenceHub, 10);
			tix.Dispose();
			plr.TemporaryData.Add("replacedscp", plr.Role.ToString());

			SwapManager.SCPsToReplace--;
			scpCooldown.AddToOrReplaceValue(plr.UserId, RoundRestart.UptimeRounds + SwapManager.ReplaceBaseCooldownRounds);
		}
	}
}
