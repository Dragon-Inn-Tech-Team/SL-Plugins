using CustomCommands.Core;
using CustomCommands.Features.SCPSwap;
using HarmonyLib;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.RoleAssign;
using System;


namespace CustomCommands.Features.SCP079Removal
{
	public class SCP079Removal : CustomFeature
	{
		public static int SCPCount = 0;

		public SCP079Removal(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerWaitingForPlayers()
		{
			SCPCount = 0;
		}

		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
		{
			//Logger.Info($"{ev.Player.Role} {ev.Player.Role == RoleTypeId.Scp079} | {SCPCount} {SCPCount > 3}");

			if (ev.Player.Role == RoleTypeId.Scp079 && SCPCount > 3)
			{
				Logger.Info($"Swapping 079 to alternate SCP");
				SCPSwap.SCPSwap.SwapHumanToScp(ev.Player, true, true, false);
				//ev.Player.SetRole(RoleTypeId.Scp3114, RoleChangeReason.LateJoin);
			}
		}
	}

	[HarmonyPatch(typeof(ScpSpawner))]
	[HarmonyPatch("SpawnScps")]
	public class ScpSpawnerPatch
	{
		[HarmonyPrefix]
		public static void prefix(int targetScpNumber)
		{
			//Logger.Info($"SCP Target number set to {targetScpNumber}");
			SCP079Removal.SCPCount = targetScpNumber;
		}
	}
}
