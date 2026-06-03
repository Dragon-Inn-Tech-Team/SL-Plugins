using CustomCommands.Core.Weapons;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using RedRightHand;

namespace CustomCommands.Features.Weapons.Weapons
{
	public class FlashLauncher : CustomWeaponBase
	{
		public override ItemType ItemType => ItemType.GunCOM15;

		public override string Name => "Flashbang Launcher";

		public override void ShootWeapon(Player player)
		{
			Helpers.SpawnGrenade<FlashbangGrenade>(player, ItemType.GrenadeFlash, Helpers.RandomThrowableVelocity(player.Camera.transform));
		}
	}
}
