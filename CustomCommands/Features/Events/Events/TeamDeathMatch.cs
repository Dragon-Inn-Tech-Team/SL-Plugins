using CustomCommands.Features.Custom.Event;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp106;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.EventRounds.Events
{
	public class TeamDeathMatch : KillfeedEvent
	{
		int ChaosKills = 0;
		int NTFKills = 0;

		public override void OnServerRoundStarted()
		{
			ChaosKills = 0;
			NTFKills = 0;

			Vector3 NtfSpawn = Vector3.zero;
			Vector3 ChaosSpawn = Vector3.zero;

			int index = 0;
			
			foreach (Player plr in Player.GetAll())
			{
				if (plr.IsHost || plr.Role == RoleTypeId.Overwatch)
					continue;

				index++;

				if (index % 2 == 0)
				{
					plr.SetRole(RoleTypeId.NtfCaptain);
					GiveLoadout(plr);

					if (NtfSpawn == Vector3.zero)
						NtfSpawn = GetSafeSpawn(plr.ReferenceHub.roleManager.CurrentRole as IFpcRole);

					plr.Position = NtfSpawn;
				}
				else
				{
					plr.SetRole(RoleTypeId.ChaosMarauder);
					GiveLoadout(plr);

					if (ChaosSpawn == Vector3.zero)
					{
						ChaosSpawnRoll:
						ChaosSpawn = GetSafeSpawn(plr.ReferenceHub.roleManager.CurrentRole as IFpcRole);

						if (Vector3.Distance(NtfSpawn, ChaosSpawn) < 30)
							goto ChaosSpawnRoll;
					}

					plr.Position = ChaosSpawn;
				}
			}

			Server.SendBroadcast($"First team to 75 kill wins", 5);
		}

		public Vector3 GetSafeSpawn(IFpcRole role)
		{
			var posesForZone = Scp106PocketExitFinder.GetPosesForZone(FacilityZone.HeavyContainment);
			var randomPose = Scp106PocketExitFinder.GetRandomPose(posesForZone);
			var raycastRange = Scp106PocketExitFinder.GetRaycastRange(FacilityZone.HeavyContainment);
			return SafeLocationFinder.GetSafePositionForPose(randomPose, raycastRange, role.FpcModule.CharController, true);
		}

		public bool IsValidSpawnRoom(Vector3 spawnPos, Team spawnTeam)
		{
			var room = Room.GetRoomAtPosition(spawnPos);
			if (room.Players.Any(p => p.Team != spawnTeam))
				return false;
			

			foreach (var adjRoom in room.AdjacentRooms)
				if (adjRoom.Players.Any(p => p.Team != spawnTeam))
					return false;	

			return true;
		}

		public override void OnPlayerDying(PlayerDyingEventArgs ev)
		{
			var roleCache = ev.Player.Role;

			if (ev.Attacker != null && ev.Attacker.RoleBase.Team != ev.Player.RoleBase.Team)
			{
				if (ev.Attacker.RoleBase.Team == Team.ChaosInsurgency)
					ChaosKills += 1;
				else if (ev.Attacker.RoleBase.Team == Team.FoundationForces)
					NTFKills += 1;

				AddKillfeedEntry(CreateEntry(ev));
			}

			MEC.Timing.CallDelayed(5, () =>
			{
				ev.Player.SetRole(roleCache, RoleChangeReason.RespawnMiniwave);
				GiveLoadout(ev.Player);

				rollRoom:
				IFpcRole fpcRole = ev.Player.ReferenceHub.roleManager.CurrentRole as IFpcRole;
				var SpawnPos = GetSafeSpawn(fpcRole);
				if (!IsValidSpawnRoom(SpawnPos, ev.Player.RoleBase.Team))
					goto rollRoom;

				ev.Player.Position = SpawnPos;
			});
		}

		public override bool CheckEndConditions()
		{
			return ChaosKills > 74 || NTFKills > 74;
		}


		public override void UpdateKillfeed()
		{
			string killfeedString = $"<size=-14><align=left><pos=-8em><color=blue>{NTFKills}</color> : <color=green>{ChaosKills}</color></align></pos></size>\n";
			foreach (KillfeedEntry entry in KillfeedEntries)
				if((Round.Duration - entry.EntryTime).TotalSeconds < 30)
					killfeedString += $"<size=-14><align=left><pos=-8em>{entry.EntryMessage}</align></pos></size>\n";

			Server.ClearBroadcasts();
			Server.SendBroadcast(killfeedString, 5);
		}

		public void GiveLoadout(Player player)
		{
			player.ClearInventory();

			//player.AddItem(ItemType.GunFRMG0); //5.56
			player.AddItem(ItemType.GunE11SR); //5.56
			player.AddAmmo(ItemType.Ammo556x45, 120);
			player.AddItem(ItemType.ArmorCombat);
			player.AddItem(ItemType.GrenadeHE);
			player.AddItem(ItemType.GrenadeHE);
			player.AddItem(ItemType.Adrenaline);
		}
	}
}
