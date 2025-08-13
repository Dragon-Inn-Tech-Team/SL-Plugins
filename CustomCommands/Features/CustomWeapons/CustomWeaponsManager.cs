using CustomCommands.Core;
using CustomCommands.Features.CustomWeapons.Weapons;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerRoles;
using System.Collections.Generic;

namespace CustomCommands.Features.CustomWeapons
{
	public class CustomWeaponsManager : CustomFeature
	{
		public enum CustomWeaponType
		{
			NONE, Grenade, Flashbang, Ball, Ragdoll, Capybara, Confetti
		}


		public static RoleTypeId[] RagdollRoles = new RoleTypeId[]
		{
					RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRepressor,
					RoleTypeId.NtfCaptain, RoleTypeId.NtfSpecialist, RoleTypeId.NtfPrivate, RoleTypeId.NtfSergeant, RoleTypeId.Tutorial
		};

		public static Dictionary<string, CustomWeaponType> EnabledWeapons = new Dictionary<string, CustomWeaponType>();
		public static Dictionary<CustomWeaponType, CustomWeaponBase> AvailableWeapons = new Dictionary<CustomWeaponType, CustomWeaponBase>();

		public CustomWeaponsManager(bool configSetting) : base(configSetting)
		{
			if (isEnabled)
			{
				AvailableWeapons = new Dictionary<CustomWeaponType, CustomWeaponBase>
				{
					{ CustomWeaponType.Grenade,  new GrenadeLauncher() },
					{ CustomWeaponType.Flashbang, new FlashLauncher() },
					{ CustomWeaponType.Ball, new BallLauncher() },
					{ CustomWeaponType.Ragdoll, new RagdollLauncher() },
					{ CustomWeaponType.Capybara, new CapybaraGun() },
					{ CustomWeaponType.Confetti, new ConfettiLauncher() },
				};
			}
		}

		public override void OnPlayerShootingWeapon(PlayerShootingWeaponEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Player.UserId, out CustomWeaponType type) && AvailableWeapons.TryGetValue(type, out var wepBase))
			{
				wepBase.ShootWeapon(ev.Player);
			}
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Player.UserId, out CustomWeaponType type) && AvailableWeapons.TryGetValue(type, out var wepBase))
			{
				wepBase.HitPlayer(ev);
			}
		}

		public override void OnPlayerPlacedBulletHole(PlayerPlacedBulletHoleEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Player.UserId, out CustomWeaponType type) && AvailableWeapons.TryGetValue(type, out var wepBase))
			{
				wepBase.PlaceBulletHole(ev);
			}
		}

		public override void OnServerWaitingForPlayers()
		{
			EnabledWeapons.Clear();
		}

	}
}
