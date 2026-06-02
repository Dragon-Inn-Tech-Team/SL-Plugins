using CustomCommands.Core;

using LabApi.Events.Arguments.PlayerEvents;
using PlayerRoles;
using PlayerStatsSystem;
using System.Collections.Generic;
using CustomCommands.Features.Custom.Weapons.Weapons;

namespace CustomCommands.Features.Custom.Weapons
{
	public class CustomWeaponsManager : CustomFeatureBase
	{
		public static RoleTypeId[] RagdollRoles = new RoleTypeId[]
		{
			RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRepressor,
			RoleTypeId.NtfCaptain, RoleTypeId.NtfSpecialist, RoleTypeId.NtfPrivate, RoleTypeId.NtfSergeant, RoleTypeId.Tutorial
		};

		public static Dictionary<ushort, CustomWeaponBase> EnabledWeapons = new Dictionary<ushort, CustomWeaponBase>();
		public static Dictionary<string, CustomWeaponBase> AvailableWeapons = new Dictionary<string, CustomWeaponBase>();

		public CustomWeaponsManager(bool configSetting) : base(configSetting)
		{
			if (IsEnabled)
			{
				AvailableWeapons = new Dictionary<string, CustomWeaponBase>
				{
					{ "grenade" , new GrenadeLauncher() },
					{ "flash",new FlashLauncher() },
					{ "ball",  new BallLauncher() },
					{ "ragdoll", new RagdollLauncher() },
					{ "capybara", new CapybaraGun() },
					{ "confetti", new ConfettiLauncher() },
				};
			}
		}

		public override void OnPlayerShootingWeapon(PlayerShootingWeaponEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.FirearmItem.Serial, out CustomWeaponBase wepBase))
			{
				wepBase.ShootWeapon(ev.Player);
			}
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (ev.DamageHandler is FirearmDamageHandler fDH && EnabledWeapons.TryGetValue(ev.Attacker.CurrentItem.Serial, out CustomWeaponBase wepBase))
			{
				wepBase.HitPlayer(ev);
			}
		}

		public override void OnPlayerPlacedBulletHole(PlayerPlacedBulletHoleEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Player.CurrentItem.Serial, out CustomWeaponBase wepBase))
			{
				wepBase.PlaceBulletHole(ev);
			}
		}

		public override void OnPlayerReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Player.CurrentItem.Serial, out CustomWeaponBase wepBase))
			{
				wepBase.ReloadingWeapon(ev);
			}
		}

		public override void OnPlayerSearchedPickup(PlayerSearchedPickupEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Pickup.Serial, out CustomWeaponBase wepBase))
			{
				wepBase.WeaponPickedUp(ev);
			}
		}

		public override void OnPlayerChangedItem(PlayerChangedItemEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.NewItem.Serial, out CustomWeaponBase wepBase))
			{
				wepBase.WeaponEquipped(ev);
			}
		}

		public override void OnServerWaitingForPlayers()
		{
			EnabledWeapons.Clear();
		}

	}
}
