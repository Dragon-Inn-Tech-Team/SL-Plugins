using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.CustomWeapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomWeapons.Weapons
{
	public class BallLauncher : CustomWeaponBase
	{
		public override string Name => "SCP-018 Launcher";
		public override ItemType Model => ItemType.GunFRMG0;

		public override int MaxMagazineAmmo => 3;

		public override void ShootWeapon(PlayerShootingWeaponEventArgs ev)
		{
			Helpers.SpawnGrenade<InventorySystem.Items.ThrowableProjectiles.Scp018Projectile>(ev.Player, ItemType.SCP018, Helpers.RandomThrowableVelocity(ev.Player.Camera.transform));
			ev.IsAllowed = false;
		}

		
	}
}
