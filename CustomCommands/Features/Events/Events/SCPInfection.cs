using CustomCommands.Features.Custom.Event;
using CustomPlayerEffects;
using InventorySystem.Items.Firearms.Attachments;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using RedRightHand;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Features.EventRounds.Events
{
	public class SCPInfection : CustomEventRound
	{
		MEC.CoroutineHandle FlickerRoutine;
		MEC.CoroutineHandle EndRoundRoutine;

		public override void OnServerRoundStarted()
		{
			RoundTimeLimit = new TimeSpan(0, 6, 0);
			int scps = Mathf.Clamp((int)Math.Floor((double)(Player.Count / 5)), 1, 5);
			int remainingPlayers = Player.Count - 1;
			var rand = new System.Random();

			var infectedSpawn = Room.Get(RoomName.Hcz939).First();
			var humanSpawn = Room.Get(RoomName.HczServers).First();

			foreach (Player plr in Player.GetAll())
			{
				if (plr.IsHost || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				if ((scps > 0 && rand.Next(0, 2) == 1) || remainingPlayers < scps)
				{
					plr.SendBroadcast("You will spawn in 10 seconds", 10, shouldClearPrevious: true);
					plr.SetRole(PlayerRoles.RoleTypeId.Tutorial);
					scps--;

					Timing.CallDelayed(10, () =>
					{
						SpawnPlayerAsInfected(plr);
					});
				}
				else
				{
					plr.SetRole(PlayerRoles.RoleTypeId.NtfSergeant);
					plr.ClearInventory();

					plr.AddItem(ItemType.ArmorHeavy);
					plr.AddFirearmWithAttachments(ItemType.GunShotgun, AttachmentName.HoloSight, AttachmentName.Flashlight);
					plr.AddAmmo(ItemType.Ammo12gauge, 74);
					plr.AddFirearmWithAttachments(ItemType.GunCOM18, AttachmentName.HoloSight, AttachmentName.Flashlight);
					plr.AddAmmo(ItemType.Ammo9x19, 210);

					plr.Position = humanSpawn.Position + Vector3.up;
					plr.SendBroadcast("The infected will release in 10 seconds", 10, shouldClearPrevious: true);
				}
				remainingPlayers--;
			}

			FlickerRoutine = Timing.CallPeriodically(60 * 8f, 30f, () =>
			{
				foreach (var a in Room.Get(FacilityZone.HeavyContainment))
				{
					a.LightController.FlickerLights(5f);
				}
			});
		}
		public override void OnPlayerDying(PlayerDyingEventArgs ev)
		{
			if (ev.Player.IsHuman)
				ev.Player.ClearInventory();

			int delay = Player.GetAll().Count(p => p.Role == PlayerRoles.RoleTypeId.Scp0492) > 5 ? 5 : 10;

			ev.Player.SendBroadcast($"You will respawn in {delay} seconds", (ushort)delay);
			Timing.CallDelayed(delay, () =>
			{
				SpawnPlayerAsInfected(ev.Player);
			});
		}

		public override void OnPlayerDeath(PlayerDeathEventArgs ev)
		{
			var humans = Player.GetAll().Where(p => p.IsHuman);
			if (humans.Count() == 1)
			{
				humans.First().MaxHealth = 150;
				humans.First().Health = 150;
				humans.First().EnableEffect<DamageReduction>(75, 60 * 8);
				humans.First().SendBroadcast("You are the last human left", 4);
			}
		}


		public void SpawnPlayerAsInfected(Player player)
		{
			player.SetRole(PlayerRoles.RoleTypeId.Scp0492, PlayerRoles.RoleChangeReason.Revived, PlayerRoles.RoleSpawnFlags.AssignInventory);
			player.Position = Room.Get(RoomName.Hcz939).First().Position + Vector3.up;
			player.Health = 75;
			player.EnableEffect<MovementBoost>(25, 60 * 8);
		}

		public override bool EventEndCondition()
		{
			return !Player.GetAll().Any(p => p.IsHuman);
		}
	}
}
