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
	public class GrenadeLauncher : CustomWeaponBase
	{
		public override string Name => "Grenade Launcher";

		public override void ShootWeapon(PlayerShootingWeaponEventArgs ev)
		{
			Helpers.SpawnGrenade<TimeGrenade>(ev.Player, ItemType.GrenadeHE, Helpers.RandomThrowableVelocity(ev.Player.Camera.transform));
		}
	}
}
