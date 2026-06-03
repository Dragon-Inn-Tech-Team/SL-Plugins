using LabApi.Features.Wrappers;
using System.Linq;
using PlayerRoles;
using RedRightHand;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerStatsSystem;
using CustomPlayerEffects;
using LabApi.Features.Console;
using CustomCommands.Core.Event;

namespace CustomCommands.Features.CustomEvents.Events
{
	//Everyone spawns with a revolve with 1 bullet.
	//Every time they shoot and hit someone, they gain 1 bullet.
	public class OneInTheChamber : KillfeedEvent
	{
		public override void OnServerRoundStarted()
		{
			Server.SendBroadcast($"Everyone is invincible for 20 seconds", 20);
			foreach (Player plr in Player.GetAll())
			{
				plr.SetRole(RoleTypeId.ClassD);
				plr.MaxHealth = 100;
				plr.Health = 100;
				plr.Position = plr.GetSafeSpawn();
				plr.EnableEffect<MovementBoost>(30, 20);
				plr.ClearInventory();
				plr.AddItem(ItemType.SCP1509);
				var gun = plr.AddItem(ItemType.GunRevolver);
				if (gun is FirearmItem firearm)
				{
					firearm.StoredAmmo = 1;
					firearm.AttachmentsCode = 0;
				}
			}
		}

		public override void OnPlayerShootingWeapon(PlayerShootingWeaponEventArgs ev)
		{
			if (Round.Duration.TotalSeconds < 20)
				ev.IsAllowed = false;

		}

		public override void OnPlayerProcessingScp1509Message(PlayerProcessingScp1509MessageEventArgs ev)
		{
			if (Round.Duration.TotalSeconds < 20)
				ev.IsAllowed = false;
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (Round.Duration.TotalSeconds < 20)
				ev.IsAllowed = false;
			else if(ev.DamageHandler is FirearmDamageHandler fDH && fDH.Firearm.ItemTypeId == ItemType.GunRevolver)
			{
				fDH.Damage = 250f;
			}
		}

		public override void OnPlayerChangedItem(PlayerChangedItemEventArgs ev)
		{
			if (ev.NewItem is FirearmItem firearm && firearm.Type == ItemType.GunRevolver && (ev.Player.Ammo.ContainsKey(ItemType.Ammo44cal) || ev.Player.Ammo[ItemType.Ammo44cal] > 0))
			{
				firearm.Reload();
			}
		}

		public override void OnPlayerDying(PlayerDyingEventArgs ev)
		{
			if (ev.DamageHandler is AttackerDamageHandler aDH)
			{
				if (Round.Duration.TotalSeconds > 20)
				{
					ev.Player.ClearInventory();
					
					if(ev.Attacker.CurrentItem is FirearmItem fi)
					{
						ev.Attacker.AddAmmo(ItemType.Ammo44cal, 1);
						fi.Reload();
					}
					else if (ev.Attacker.Ammo[ItemType.Ammo44cal] == 0)
						ev.Attacker.AddAmmo(ItemType.Ammo44cal, 1);
				}
				else
					ev.IsAllowed = false;

				AddKillfeedEntry(CreateEntry(ev));
			}
		}

		public override bool CheckEndConditions() => Player.GetAll().Count(p => p.Role == RoleTypeId.ClassD) < 2;

		public override void OnPlayerScp1509Resurrecting(PlayerScp1509ResurrectingEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerUnloadingWeapon(PlayerUnloadingWeaponEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerSearchingAmmo(PlayerSearchingAmmoEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerPickingUpAmmo(PlayerPickingUpAmmoEventArgs ev)
		{
			ev.IsAllowed = false;
		}
	}
}
