using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using RedRightHand;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.SCPSwap
{
	public class SCPSwap : CustomFeatureBase
	{


		public static bool SwapPaused = false;

		public static int SwapSeconds { get; internal set; } = 60;

		public static int SCPsToReplace = 0;
		static int ReplaceBaseCooldownRounds = 3;

		static Dictionary<string, int> triggers = new Dictionary<string, int>();
		static Dictionary<string, int> scpCooldown = new Dictionary<string, int>();
		private static readonly RoleTypeId[] scpRoles = { RoleTypeId.Scp049, RoleTypeId.Scp079, RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939, RoleTypeId.Scp096 };

		public class Raffle
		{
			/// <summary>
			/// Player ID, Raffle weighting
			/// </summary>
			private Dictionary<int, uint> raffleParticipants = new Dictionary<int, uint>();
			public bool IsActive { get; private set; } = true;

			public Raffle() { IsActive = false; }
			public Raffle(int PlayerID, uint Weighting, float RaffleTime = 5f)
			{
				_ = AddParticipant(PlayerID, Weighting);
				startRaffle(RaffleTime);
			}
			public bool AddParticipant(int PlayerID, uint Weighting)
			{
				if (IsActive && raffleParticipants.ContainsKey(PlayerID)) return false;
				raffleParticipants.Add(PlayerID, Weighting);
				return true;
			}
			private void startRaffle(float time)
			{
				MEC.Timing.CallDelayed(time, () =>
				{
					bool NwMoment = false;
					int draw = -1;
					List<int> winners = new List<int>();
					if (raffleParticipants.Count == 0) return;
					List<KeyValuePair<int, uint>> DrawGroup = raffleParticipants.ToList();
					if (raffleParticipants.Count >= 6)
					{
						DrawGroup.Sort((x, y) => x.Value.CompareTo(y.Value));
						DrawGroup = raffleParticipants.Skip(DrawGroup.Count / 2).ToList();
					}
					DrawGroup.ShuffleList();

					redraw: //Makes sure the person didn't leave in the 5 second draw time and that all SCP slots are filled
					if (DrawGroup.Count == 0)
					{
						finishRaffle(winners);
						return;
					}

					draw = DrawGroup.First().Key;
					DrawGroup.RemoveAt(0);

					retryPlayer:

					try
					{
						if (Player.TryGet(draw, out var drawPlr))
						{
							SwapHumanToScp(drawPlr);
							winners.Add(draw);
							NwMoment = false;
						}
						else goto redraw;
					}
					catch (Exception e)
					{
						Logger.Error(e.Message);
						if (NwMoment)
						{
							Logger.Info($"Skipping raffle participant with ID {draw}");
							NwMoment = false;
							goto redraw;
						}
						else
						{
							NwMoment = true;
							goto retryPlayer;
						}
					}

					if (SCPsToReplace == 0)
						finishRaffle(winners);
					else goto redraw;
				});
			}
			private void finishRaffle(List<int> winners)
			{
				while (raffleParticipants.Count > 0)
				{
					try
					{
						if (!winners.Contains(raffleParticipants.Last().Key))
							if (Player.TryGet(raffleParticipants.Last().Key, out var plr))
								plr.SendHint("You lost the raffle.", 3);
					}
					catch (Exception e) { Logger.Error(e.Message); }
					raffleParticipants.Remove(raffleParticipants.Last().Key);
				}
				IsActive = false;
			}
		}

		public static Raffle raffle;

		public static RoleTypeId[] AvailableSCPs
		{
			get
			{
				var tempRoles = scpRoles.ToList();
				var currentScpRoles = Player.List.Where(r => r.ReferenceHub.IsSCP(false)).Select(r => r.Role);

				if (!currentScpRoles.Any())
				{
					_ = tempRoles.Remove(RoleTypeId.Scp079);
					_ = tempRoles.Remove(RoleTypeId.Scp096);
				}

				foreach (var r in currentScpRoles)
				{
					_ = tempRoles.Remove(r);

					if (r == RoleTypeId.Scp079 || r == RoleTypeId.Scp096) // 079/096 exclusivity
					{
						_ = tempRoles.Remove(RoleTypeId.Scp079);
						_ = tempRoles.Remove(RoleTypeId.Scp096);
					}
				}

				if (tempRoles.Any()) // Incase this is used on a really big server
					return tempRoles.ToArray();
				else
					return scpRoles;
			}
		}

		public SCPSwap(bool configSetting) : base(configSetting)
		{
		}

		public static void ReplaceBroadcast()
		{
			Server.ClearBroadcasts();
			Server.SendBroadcast($"There {(SCPsToReplace == 1 ? "is" : "are")} now {SCPsToReplace} SCP spot{(SCPsToReplace == 1 ? "" : "s")} available.\nHit the SCP swap keybind to queue for an SCP", 5);
		}

		public static bool CanHumanSwapToScp(ReferenceHub plr, out string reason) => CanHumanSwapToScp(Player.Get(plr), out reason);
		public static bool CanHumanSwapToScp(Player plr, out string reason)
		{
			if (!CustomCommandsPlugin.Config.EnableScpSwap)
			{
				reason = "Swapping is not enabled on this server";
				return false;
			}
			if (SwapPaused)
			{
				reason = "Swapping has been paused";
				return false;
			}
			if (SCPsToReplace < 1)
			{
				reason = "There are no SCPs to replace";
				return false;
			}
			if (Round.Duration > TimeSpan.FromSeconds(SwapSeconds))
			{
				reason = $"You can only replace an SCP within the first {SwapSeconds} seconds of the round";
				return false;
			}
			if (scpCooldown.TryGetValue(plr.UserId, out int roundCount))
			{
				if (roundCount > RoundRestart.UptimeRounds)
				{
					if (SCPSwap.triggers.TryGetValue(plr.UserId, out int count))
					{
						if (count > 2)
						{
							SCPSwap.scpCooldown[plr.UserId]++;
							SCPSwap.triggers[plr.UserId] = 0;
						}
						else SCPSwap.triggers[plr.UserId]++;
					}
					else
						SCPSwap.triggers.Add(plr.UserId, 1);


					reason = $"You are on cooldown for another {SCPSwap.scpCooldown[plr.UserId] - RoundRestart.UptimeRounds} round(s).";
					return false;
				}
			}

			reason = string.Empty;
			return true;
		}

		public static void QueueSwapHumanToScp(ReferenceHub plr) => QueueSwapHumanToScp(Player.Get(plr));
		public static void QueueSwapHumanToScp(Player plr)
		{
			ScpTicketsLoader tix = new ScpTicketsLoader();
			int numTix = tix.GetTickets(plr.ReferenceHub, 10);
			tix.Dispose();
			uint rGroup = 1;
			if (numTix >= 13) rGroup = (uint)numTix;
			if (plr.Role == RoleTypeId.Spectator) rGroup <<= 8;

			if (raffle != null && raffle.IsActive)
			{
				if (raffle.AddParticipant(plr.PlayerId, rGroup))
					plr.SendHint("You are now in the SCP raffle", 3);
			}
			else
			{
				raffle = new Raffle(plr.PlayerId, rGroup);
				plr.SendHint("You are now in the SCP raffle", 3);
			}
		}

		public static void SwapHumanToScp(ReferenceHub plr, bool include3114 = false, bool exclude079 = false, bool isSwap = true) => SwapHumanToScp(Player.Get(plr), include3114, exclude079, isSwap);
		public static void SwapHumanToScp(Player plr, bool include3114 = false, bool exclude079 = false, bool isSwap = true)
		{
			List<RoleTypeId> scps = SCPSwap.AvailableSCPs.ToList();
			if (include3114)
				scps.Add(RoleTypeId.Scp3114);
			if (exclude079)
				scps.Remove(RoleTypeId.Scp079);


			plr.SetRole(scps[new Random().Next(0, scps.Count)], RoleChangeReason.LateJoin);
			ScpTicketsLoader tix = new ScpTicketsLoader();
			tix.ModifyTickets(plr.ReferenceHub, 10);
			tix.Dispose();

			if (isSwap)
			{
				SCPSwap.SCPsToReplace--;
				scpCooldown.AddToOrReplaceValue(plr.UserId, RoundRestart.UptimeRounds + SCPSwap.ReplaceBaseCooldownRounds);
			}
		}


		#region Events

		public override void OnServerRoundEnded(RoundEndedEventArgs ev)
		{
			triggers.Clear();
			SCPsToReplace = 0;
			SCPSwap.raffle = null;
		}
		public override void OnServerRoundStarted()
		{
			SCPSwap.SCPsToReplace = 0;

			Timing.CallDelayed(SCPSwap.SwapSeconds + 5f, () =>
			{
				if (SCPSwap.SCPsToReplace > 0)
				{
					var currentScps = Player.List.Where(p => p.ReferenceHub.IsSCP(false));
					if (currentScps.Count() == 1 && (currentScps.First().Role == RoleTypeId.Scp079 || currentScps.First().Role == RoleTypeId.Scp096))
					{
						var newRole = SCPSwap.scpRoles.Except([RoleTypeId.Scp079, RoleTypeId.Scp096]).ToList().RandomItem();
						currentScps.First().SetRole(newRole, RoleChangeReason.LateJoin);
					}

					foreach (var scpPlr in currentScps)
					{
						if (scpPlr.Role.IsValidSCP())
						{
							scpPlr.Health += 500 * SCPSwap.SCPsToReplace;
							scpPlr.MaxHealth += 500 * SCPSwap.SCPsToReplace;
						}
					}

					if (currentScps.Count() == 0)
						Server.SendBroadcast("There are no SCPs this round.", 5, Broadcast.BroadcastFlags.Normal, true);
					else
						Server.SendBroadcast($"Due to {SCPSwap.SCPsToReplace} missing SCP(s), All living SCPs have been buffed", 5, Broadcast.BroadcastFlags.Normal, true);

				}
			});
		}

		public override void OnPlayerLeft(PlayerLeftEventArgs ev)
		{
			if (Round.Duration < TimeSpan.FromSeconds(SwapSeconds) && ev.Player.ReferenceHub.IsSCP(false))
			{
				SCPSwap.SCPsToReplace++;
				SCPSwap.ReplaceBroadcast();
			}
		}

		#endregion
	}
}
