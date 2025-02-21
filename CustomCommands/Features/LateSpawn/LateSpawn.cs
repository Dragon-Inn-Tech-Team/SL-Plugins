﻿using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.LateSpawn
{
	public class LateSpawn : CustomFeature
	{
		public static bool LateRespawnPaused = false;
		DateTime lastRespawn;
		Team lastTeam;

		public List<Player> lateSpawnPlayers = new List<Player>();

		public LateSpawn(bool configSetting) : base(configSetting)
		{
			lastRespawn = DateTime.Now;
		}

		public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
		{
			lastRespawn = DateTime.Now;
			lastTeam = ev.Team;

			Timing.CallDelayed(CustomCommandsPlugin.Config.LateSpawnTime, () =>
			{
				foreach (var a in lateSpawnPlayers)
				{
					if (lastTeam == Team.FoundationForces)
						a.SetRole(RoleTypeId.NtfPrivate, RoleChangeReason.Respawn, RoleSpawnFlags.All);
					else
						a.SetRole(RoleTypeId.ChaosRifleman, RoleChangeReason.Respawn, RoleSpawnFlags.All);
				}

				lateSpawnPlayers.Clear();
			});
		}

		public override void OnPlayerDeath(PlayerDeathEventArgs ev)
		{
			if(!LateRespawnPaused && (DateTime.Now - lastRespawn).TotalSeconds < CustomCommandsPlugin.Config.LateSpawnTime && ev.Attacker.Team != Team.SCPs)
			{
				lateSpawnPlayers.Add(ev.Player);
			}	
		}

	}
}
