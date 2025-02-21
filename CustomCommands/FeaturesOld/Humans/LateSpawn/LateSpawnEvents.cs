﻿using MEC;
using PlayerRoles;
using LabApi.Features.Wrappers;
using LabApi.Events;
using PluginAPI.Events;
using System;

namespace CustomCommands.Features.Humans.LateSpawn
{
	public class LateSpawnEvents
	{
		public DateTime LastRespawn = new DateTime();
		public Faction LastTeam = Faction.Unclassified;

		[PluginEvent, PluginPriority(LoadPriority.Low)]
		public void RespawnEvent(TeamRespawnEvent ev)
		{
			LastRespawn = DateTime.Now;
			LastTeam = ev.Team;
		}


		[PluginEvent, PluginPriority(LoadPriority.Low)]
		public void PlayerDeath(PlayerDeathEvent ev)
		{
			//Logger.Info((DateTime.Now - LastRespawn).TotalSeconds.ToString());

			if ((DateTime.Now - LastRespawn).TotalSeconds < CustomCommandsPlugin.Config.LateSpawnTime && (ev.Attacker != null && ev.Attacker.Team != Team.SCPs))
			{
				Timing.CallDelayed(1f, () =>
				{
					if (LastTeam == Faction.FoundationStaff)
						ev.Player.SetRole(RoleTypeId.NtfPrivate);
					else if (LastTeam == Faction.FoundationEnemy)
						ev.Player.SetRole(RoleTypeId.ChaosRifleman);
				});
			}
		}
	}
}
