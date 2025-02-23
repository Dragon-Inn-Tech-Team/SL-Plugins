using CustomCommands.Core;
using LabApi.Features.Wrappers;
using PlayerRoles.RoleAssign;
using PlayerRoles;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedRightHand;
using Extensions = RedRightHand.Extensions;
using LabApi.Events.Arguments.ServerEvents;
using MEC;
using PlayerStatsSystem;
using LabApi.Events.Arguments.PlayerEvents;
using UserSettings.ServerSpecific;

namespace CustomCommands.Features.SCPSwap
{
    public class SCPSwap : CustomFeature
    {
        internal enum SwapType
        {
            SwappedToHuman,
            SwappedToSCP,
        }

        public static bool SwapPaused = false;

        static int _swapSeconds = 60;
        public static int SwapToHumanSeconds => _swapSeconds;
        public static int SwapToScpSeconds => (int)(_swapSeconds * 1.5f);

        public static int SCPsToReplace = 0;
        static int ReplaceBaseCooldownRounds = 3;

        static Dictionary<string, int> triggers = new Dictionary<string, int>();
        static Dictionary<string, int> scpCooldown = new Dictionary<string, int>();
        static Dictionary<string, int> humanCooldown = new Dictionary<string, int>();
        static Dictionary<ReferenceHub, uint> raffleParticipants = new Dictionary<ReferenceHub, uint>();
        static Dictionary<string, string> swapRequests = new Dictionary<string, string>();
        static Dictionary<string, SwapType> swapDict = new Dictionary<string, SwapType>();
        private static List<RoleTypeId> scpRoles = new List<RoleTypeId>() { RoleTypeId.Scp049, RoleTypeId.Scp079, RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939, RoleTypeId.Scp096 };


        public static RoleTypeId[] AvailableSCPs
        {
            get
            {
                var tempRoles = scpRoles;
                var CurrentSCPRoles = Player.List.Where(r => r.ReferenceHub.IsSCP()).Select(r => r.Role);

                foreach (var r in CurrentSCPRoles)
                {
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

        public SCPSwap(bool configSetting) : base(configSetting)
        {
        }

        public static void ReplaceBroadcast()
        {
            Server.ClearBroadcasts();
            Server.SendBroadcast($"There {(SCPsToReplace == 1 ? "is" : "are")} now {SCPsToReplace} SCP spot{(SCPsToReplace == 1 ? "" : "s")} available. Run \".scp\" to queue for an SCP", 5);
        }

        public static bool CanScpSwapToHuman(ReferenceHub plr, out string reason) => CanScpSwapToHuman(Player.Get(plr), out reason);
        public static bool CanScpSwapToHuman(Player plr, out string reason)
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
            if (swapDict.ContainsKey(plr.UserId))
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
            if (swapDict.ContainsKey(plr.UserId))
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

        public static void SwapScpToHuman(ReferenceHub plr) => SwapScpToHuman(Player.Get(plr));
        public static void SwapScpToHuman(Player plr)
        {
            SCPSwap.SCPsToReplace++;
            HumanSpawner.SpawnLate(plr.ReferenceHub);
            swapDict.AddToOrReplaceValue(plr.UserId, SwapType.SwappedToHuman);
            SCPSwap.ReplaceBroadcast();

            humanCooldown.AddToOrReplaceValue(plr.UserId, RoundRestart.UptimeRounds + SCPSwap.ReplaceBaseCooldownRounds);
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
                plr.SendHint("You are now in the SCP raffle", 3);

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
                                temp.SendHint("You lost the raffle.", 3);
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
            var scps = SCPSwap.AvailableSCPs;

            plr.SetRole(scps[new Random().Next(0, scps.Length)], RoleChangeReason.LateJoin);
            ScpTicketsLoader tix = new ScpTicketsLoader();
            tix.ModifyTickets(plr.ReferenceHub, 10);
            tix.Dispose();
            swapDict.AddToOrReplaceValue(plr.UserId, SwapType.SwappedToSCP);

            SCPSwap.SCPsToReplace--;
            scpCooldown.AddToOrReplaceValue(plr.UserId, RoundRestart.UptimeRounds + SCPSwap.ReplaceBaseCooldownRounds);
        }

        public static bool HasSwapRequest(Player plr, out Player swapper)
        {
            swapper = null;
            return swapRequests.TryGetValue(plr.UserId, out var swapperID) && Player.TryGet(swapperID, out swapper) ||
                swapRequests.Values.Where(r => r == plr.UserId).Any();
        }
        public static void RemoveSwapRequest(Player plr)
        {
            if (swapRequests.ContainsKey(plr.UserId))
                swapRequests.Remove(plr.UserId);
        }
        public static void AddSwapRequest(Player player, Player target)
        {
            //This is reversed so that you can easily check from the target side if they have any pending requests;
            swapRequests.AddToOrReplaceValue(target.UserId, player.UserId);

            target.SendHint($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to {player.Role}, or type `.sswapd` to reject the request", 8);
            target.SendConsoleMessage($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to SCP-{player.Role}, or type `.sswapd` to reject the request");
        }
        public static bool CanSwap(Player player, Player target, out string response)
        {
            if (player.Health != player.MaxHealth)
            {
                response = "You cannot swap as you have taken damage";
                return false;
            }
            else if (target != null && target.Health != target.MaxHealth)
            {
                response = "You cannot swap as the person you want to swap with has taken damage";
                return false;
            }
            else if (Round.Duration > TimeSpan.FromMinutes(1))
            {
                response = "You can only swap your SCP within the first minute of a round";
                return false;
            }

            response = string.Empty;
            return true;
        }

        public static void SwapSCPs(Player player, Player target)
        {
            RoleTypeId playerSCP = player.Role;
            RoleTypeId targetSCP = target.Role;

            player.SetRole(targetSCP, RoleChangeReason.LateJoin);
            target.SetRole(playerSCP, RoleChangeReason.LateJoin);

            RemoveSwapRequest(player);
        }

        #region Events

        public override void OnServerRoundEnded(RoundEndedEventArgs ev)
        {
            triggers.Clear();
            swapDict.Clear();
            swapRequests.Clear();
            SCPsToReplace = 0;
        }
        public override void OnServerRoundStarted()
        {
            SCPSwap.SCPsToReplace = 0;

            Timing.CallDelayed(SCPSwap.SwapToHumanSeconds, () =>
            {
                if (SCPSwap.SCPsToReplace > 0)
                {
                    foreach (var scpPlr in Player.List.Where(p => p.IsSCP))
                    {
                        if (scpPlr.Role.IsValidSCP())
                        {
                            scpPlr.GetStatModule<HealthStat>().CurValue = scpPlr.MaxHealth + (500 * SCPSwap.SCPsToReplace);
                        }
                    }

                    Server.SendBroadcast($"Due to {SCPSwap.SCPsToReplace} missing SCP(s), All living SCPs have been buffed", 5, Broadcast.BroadcastFlags.Normal, true);
                }
            });
        }
        public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
        {
            if (ev.Player.Role.IsValidSCP() && Round.Duration < TimeSpan.FromMinutes(1))
            {
                if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSTwoButtonsSetting>(ev.Player.ReferenceHub, (int)SettingsIDs.SCP_NeverSCP, out SSTwoButtonsSetting settings)
                    && settings.SyncIsA)
                {
                    Timing.CallDelayed(0.15f, () =>
                    {
                        SCPSwap.SwapScpToHuman(ev.Player.ReferenceHub);
                    });
                }
                else
                {
                    ev.Player.SendBroadcast("You can swap SCP with another player by running the \".scpswap <SCP>\" command in your console", 5);
                    ev.Player.SendBroadcast("You can change back to a human role by running the \".human\" command", 5);
                }
            }
        }
        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            if (Round.Duration < TimeSpan.FromMinutes(1) && ev.Player.IsSCP)
            {
                SCPSwap.SCPsToReplace++;
                SCPSwap.ReplaceBroadcast();
            }
        }

        #endregion
    }
}
