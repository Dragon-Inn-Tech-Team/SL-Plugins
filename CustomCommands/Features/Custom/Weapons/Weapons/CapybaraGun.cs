using CustomCommands.Core;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using CapybaraToy = LabApi.Features.Wrappers.CapybaraToy;

namespace CustomCommands.Features.Custom.Weapons.Weapons
{
	public class CapybaraGun : CustomWeaponBase
	{
		public override string Name => "CapybaraGun";
		public override ItemType ItemType => ItemType.GunCom45;

		public override void ShootWeapon(Player player)
		{
			var capy = CapybaraToy.Create(player.Camera.position, player.Camera.rotation);
			capy.Scale = new(0.25f, 0.25f, 0.25f);

			var rb = capy.GameObject.AddComponent<Rigidbody>();
			rb.excludeLayers = LayerMask.GetMask("Player", "Viewmodel", "InvisibleCollider", "Hitbox"); // still scuffed
			rb.AddForce(player.Camera.forward * 10f, ForceMode.VelocityChange);

			_ = capy.GameObject.AddComponent<NetworkRigidbodyUnreliable>();
			capy.GameObject.AddComponent<SelfDestructProjectile>().DelayedDestroy(5f);
		}
	}
}
