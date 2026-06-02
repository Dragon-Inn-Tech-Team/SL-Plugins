using CustomCommands.Features.Custom.Event;
using LabApi.Features.Wrappers;
using System.Linq;
using PlayerRoles;
using RedRightHand;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerStatsSystem;
using CustomPlayerEffects;
using LabApi.Features.Console;

namespace CustomCommands.Features.EventRounds.Events
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
				var gun = plr.AddItem(ItemType.GunCOM18);
				if (gun is FirearmItem firearm)
				{
					firearm.StoredAmmo = 1;
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
			else if(ev.DamageHandler is FirearmDamageHandler fDH && fDH.Firearm.ItemTypeId == ItemType.GunCOM18)
			{
				fDH.Damage = 250f;
			}
		}

		public override void OnPlayerDying(PlayerDyingEventArgs ev)
		{
			if (ev.DamageHandler is AttackerDamageHandler aDH)
			{
				if (Round.Duration.TotalSeconds > 20)
				{
					ev.Player.ClearInventory();
					ev.Attacker.AddAmmo(ItemType.Ammo9x19, 1);
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
	}
}
