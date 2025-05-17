﻿using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp079Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles.FirstPersonControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Features.EventRounds.Events
{
	//Scientists vs 939 in heavy containment with all lights out.
	//They have 8 minutes to enable all 3 generators. They each have a flashlight and 05 keycard.
	//If they fail, all scientists die. If they succeed, all 939s die.
	//If you die, you die and are stuck in spectator.
	public class HeavyHideAndSeek : CustomEventRound
	{
		MEC.CoroutineHandle LoopHandle;

		public override void OnServerRoundStarted()
		{
			Round.IsLocked = true;

			LoopHandle = MEC.Timing.CallDelayed(60 * 8, () =>
			{
				foreach (var player in Player.GetAll())
				{
					if (player.Role != PlayerRoles.RoleTypeId.Scp939)
						player.Kill();
				}
			});

			var room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Hcz079);
			var room914 = room.First();

			room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Hcz939);
			var room939 = room.First();

			MEC.Timing.CallDelayed(30, () =>
			{
				foreach (var door in DoorVariant.DoorsByRoom[room914])
				{
					door.NetworkTargetState = true;
				}
			});

			int scps = Mathf.Clamp((int)Math.Floor((double)(Player.Count / 5)), 1, 5);
			int remainingPlayers = Player.Count - 1;
			var rand = new System.Random();

			foreach (Player plr in Player.GetAll())
			{
				if (plr.IsHost || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				if ((scps > 0 && rand.Next(0, 2) == 1) || remainingPlayers < scps)
				{
					plr.SetRole(PlayerRoles.RoleTypeId.Scp939);
					plr.Position = new Vector3(room914.transform.position.x, room914.transform.position.y + 1, room914.transform.position.z);
					plr.ClearBroadcasts();
					plr.SendBroadcast("The door will open in 30 seconds. Kill them all", 10, shouldClearPrevious: true);
					scps--;
				}
				else
				{
					plr.SetRole(PlayerRoles.RoleTypeId.Scientist, PlayerRoles.RoleChangeReason.RemoteAdmin, PlayerRoles.RoleSpawnFlags.None);
					plr.Position = new Vector3(room939.transform.position.x, room939.transform.position.y + 1, room939.transform.position.z);
					plr.AddItem(ItemType.Flashlight);
					plr.AddItem(ItemType.KeycardO5);
					plr.SendBroadcast("The hunters will release in 30 seconds", 10, shouldClearPrevious: true);
				}
				remainingPlayers--;
			}

			Map.TurnOffLights(100000f);
			Round.IsLocked = false;
		}

		public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
		{
			if (ev.Pickup.Category == ItemCategory.Firearm || ev.Pickup.Category == ItemCategory.Grenade || ev.Pickup.Category == ItemCategory.SCPItem || ev.Pickup.Category == ItemCategory.SpecialWeapon)
			{
				ev.Player.Damage(10, "Cease");
				ev.IsAllowed = false;
			}
		}

		public override void OnScp079Recontaining(Scp079RecontainingEventArgs ev)
		{
			base.OnScp079Recontaining(ev);
		}

		public override void OnServerRoundEnded(RoundEndedEventArgs ev)
		{
			MEC.Timing.KillCoroutines(LoopHandle);
		}
	}
}
