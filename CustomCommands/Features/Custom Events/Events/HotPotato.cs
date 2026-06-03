using AFK;
using CustomCommands.Core.Event;
using CustomPlayerEffects;
using InventorySystem.Items.Scp1509;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;

namespace CustomCommands.Features.CustomEvents.Events
{
	//Everyone is spawned as ClassD. After 30 seconds, someone is given the potato.
	//They must hit another player in order to pass the potato to them. If someone holds it for 30 seconds, they are exploded and the potato is passed to another player.
	public class HotPotato : CustomEventRound
	{
		Random Random = new();
		Dictionary<string, PotatoData> PotatoDataDict = new Dictionary<string, PotatoData>();

		public struct PotatoData
		{
			public PotatoData(TimeSpan triggerTime, string triggerType)
			{
				TriggerTime = triggerTime;
				TriggerType = triggerType;
			}

			public TimeSpan TriggerTime { get; set; }
			public string TriggerType { get; set; }
		}


		public override void OnServerRoundStarted()
		{
			PotatoDataDict.Clear();
			Server.SendBroadcast($"The potato spawns in 15 seconds", 15);
			foreach (Player plr in Player.GetAll())
			{
				plr.SetRole(RoleTypeId.ClassD);
				plr.MaxHealth = 100;
				plr.Health = 100;
				plr.Position = plr.GetSafeSpawn();
				plr.ClearInventory();
			}

			Timing.CallDelayed(15, () =>
			{
				PassPotatoToRandom();
			});
		}

		public void PassPotatoToRandom()
		{
			if (!Round.IsRoundInProgress || Round.IsRoundEnded)
				return;

			KeyValuePair<bool, Player> SelectedPlayer = new KeyValuePair<bool, Player>(false, null);
			foreach(var plr in Player.GetAll().Where(p => p.Role == RoleTypeId.ClassD))
			{
				if (OnPotatoCooldown(plr))
					continue;

				IAFKRole iafkrole = plr.ReferenceHub.roleManager.CurrentRole as IAFKRole;

				if (SelectedPlayer.Value == null)
				{
					SelectedPlayer = new KeyValuePair<bool, Player>(iafkrole.IsAFK, plr);
					continue;
				}

				if (iafkrole.IsAFK) //If this iterated player is AFK
				{
					if (!SelectedPlayer.Key || (SelectedPlayer.Key && Random.Next(10) > 6)) //If both this player and the selected player are both AFK, or the selected player is not AFK
					{
						SelectedPlayer = new KeyValuePair<bool, Player>(iafkrole.IsAFK, plr);
					}
				}
				else
				{
					if (SelectedPlayer.Key || Random.Next(10) < 7) //If the selected player is AFK, or if the player has good luck (Under 7 when rolling between 0 and 9 inclusive)
						continue;
					else
						SelectedPlayer = new KeyValuePair<bool, Player>(iafkrole.IsAFK, plr);
				}
			}

			PassPotatoToPlayer(SelectedPlayer.Value);
		}

		public void PassPotatoToPlayer(Player player, Player passer = null)
		{
			player.CurrentItem = player.AddItem(ItemType.SCP1509);
			player.EnableEffect<MovementBoost>(5, 15);
			player.SendHint("You now hold the potato");

			PotatoDataDict.AddToOrReplaceValue(player.UserId, new PotatoData(Round.Duration, "HoldingPotato"));

			if (passer != null)
			{
				passer.ClearInventory();
				passer.DisableAllEffects();
				PotatoDataDict.AddToOrReplaceValue(passer.UserId, new PotatoData(Round.Duration, "PotatoImmune"));
			}
		}

		public bool OnPotatoCooldown(Player plr) => PotatoDataDict.TryGetValue(plr.UserId, out var potData) && potData.TriggerType == "PotatoImmune" && (Round.Duration - potData.TriggerTime).TotalSeconds < 7;	

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{	
			if(ev.DamageHandler is Scp1509DamageHandler scpdh)
			{
				ev.IsAllowed = false;
				if (OnPotatoCooldown(ev.Player))
				{
					ev.Attacker.SendHint("They are currently immune to the potato");
					return;
				}
				else PassPotatoToPlayer(ev.Player, ev.Attacker);
			}
		}

		public override void OnPlayerUpdatedEffect(PlayerEffectUpdatedEventArgs ev)
		{
			if(ev.Effect is MovementBoost mbe && ev.Effect.Intensity < 1)
			{
				if (ev.Player.Items.Any(i => i.Type == ItemType.SCP1509))
				{
					ev.Player.ClearInventory();
					ExplosionUtils.ServerSpawnEffect(ev.Player.Position, ItemType.GrenadeHE);
					ev.Player.Damage(250, $"Couldn't handle the potato");		
					PotatoDataDict.Remove(ev.Player.UserId);

					Server.SendBroadcast($"The potato exploded. Picking new player", 5);
					if (!CheckEndConditions())
					{
						Timing.CallDelayed(5, () =>
						{
							PassPotatoToRandom();
						});
					}		
				}
			}
		}

		public override bool CheckEndConditions() => Player.GetAll().Count(p => p.Role == RoleTypeId.ClassD) < 2;

		public override void OnPlayerChangingItem(PlayerChangingItemEventArgs ev)
		{
			if (ev.OldItem != null && ev.OldItem.Type == ItemType.SCP1509)
				ev.IsAllowed = false;
		}
	}
}
