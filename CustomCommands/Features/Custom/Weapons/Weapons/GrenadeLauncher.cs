using CustomCommands.Core;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using RedRightHand;

namespace CustomCommands.Features.Custom.Weapons.Weapons
{
	public class GrenadeLauncher : CustomWeaponBase
	{
		public override ItemType ItemType => ItemType.GunCOM15;

		public override string Name => "Grenade Launcher";

		public override void ShootWeapon(Player player)
		{
			Helpers.SpawnGrenade<TimeGrenade>(player, ItemType.GrenadeHE, Helpers.RandomThrowableVelocity(player.Camera.transform));
		}
	}
}
