using CustomCommands.Core.Weapons;
using LabApi.Features.Wrappers;
using RedRightHand;

namespace CustomCommands.Features.Weapons.Weapons
{
	public class BallLauncher : CustomWeaponBase
	{
		public override ItemType ItemType => ItemType.GunRevolver;

		public override string Name => "SCP-018 Launcher";

		public override void ShootWeapon(Player player)
		{
			Helpers.SpawnGrenade<InventorySystem.Items.ThrowableProjectiles.Scp018Projectile>(player, ItemType.SCP018, Helpers.RandomThrowableVelocity(player.Camera.transform));
		}
	}
}
