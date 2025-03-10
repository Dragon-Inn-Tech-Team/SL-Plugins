using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using RedRightHand.Core;
using System;
using System.Linq;
using UserSettings.ServerSpecific;

namespace CustomCommands.Features.SCPs.Swap
{
	public class SwapEvents
	{
		[PluginEvent]
		public void PlayerSpawn(PlayerSpawnEvent args)
		{
			if (args.Player.Role.IsValidSCP() && Round.Duration < TimeSpan.FromMinutes(1) && !Plugin.EventInProgress)
			{
				if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSTwoButtonsSetting>(args.Player.ReferenceHub, (int)SettingsIDs.SCP_NeverSCP, out SSTwoButtonsSetting settings)
					&& settings.SyncIsA)
				{
					Timing.CallDelayed(0.15f, () =>
					{
						SwapManager.SwapScpToHuman(args.Player);
					});
				}
				else
					args.Player.SendBroadcast("You can swap SCP with another player by running the \".scpswap <SCP>\" command in your console", 5);
				args.Player.SendBroadcast("You can change to a human role by running the \".human\" command", 5);
			}
		}

		[PluginEvent]
		public void RoundStart(RoundStartEvent args)
		{
			SwapManager.SCPsToReplace = 0;

			Timing.CallDelayed(SwapManager.SwapToHumanSeconds, () =>
			{
				if(SwapManager.SCPsToReplace > 0)
				{
					var currentScps = Player.GetPlayers().Where(p => p.ReferenceHub.IsSCP(false));
					if (currentScps.Count() == 1 && (currentScps.First().Role == RoleTypeId.Scp079 || currentScps.First().Role == RoleTypeId.Scp096))
					{
                        var newRole = SwapManager.scpRoles.ToList().Except(new RoleTypeId[] { RoleTypeId.Scp079, RoleTypeId.Scp096 }).ToList().RandomItem();
						currentScps.First().SetRole(newRole, RoleChangeReason.LateJoin);
                    }

                    foreach (var scpPlr in currentScps)
					{
						if (scpPlr.Role.IsValidSCP())
						{
							scpPlr.GetStatModule<HealthStat>().CurValue = scpPlr.MaxHealth + (500 * SwapManager.SCPsToReplace);
						}
					}

					if (currentScps.Count() == 0)
                        Server.SendBroadcast("There are no SCPs this round.", 5, Broadcast.BroadcastFlags.Normal, true);
                    else
						Server.SendBroadcast($"Due to {SwapManager.SCPsToReplace} missing SCP(s), All living SCPs have been buffed", 5, Broadcast.BroadcastFlags.Normal, true);
				}
			});
		}

		[PluginEvent]
		public void RoundEnd(RoundEndEvent args)
		{
			SwapManager.SCPsToReplace = 0;
			SwapManager.triggers.Clear();
			SwapManager.raffle = null;
        }

		[PluginEvent]
		public void PlayerLeave(PlayerLeftEvent args)
		{
			if (Round.Duration < TimeSpan.FromMinutes(1) && args.Player.IsSCP)
			{
				SwapManager.SCPsToReplace++;
				SwapManager.ReplaceBroadcast();
			}
		}
	}
}
