using AdminToys;
using Interactables;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using Mirror;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CapybaraToy = LabApi.Features.Wrappers.CapybaraToy;

namespace CustomCommands.Features.CustomWeapons.Weapons
{
	public class CapybaraGun : CustomWeaponBase
	{
		public override CustomWeaponsManager.CustomWeaponType WeaponType => CustomWeaponsManager.CustomWeaponType.Capybara;

		public override string Name => "CapybaraGun";

		public override void ShootWeapon(Player player)
		{
			var capy = CapybaraToy.Create(player.Camera.position, player.Camera.rotation);
			capy.Scale = new(0.25f, 0.25f, 0.25f);
			
			var rb = capy.GameObject.AddComponent<Rigidbody>();
			rb.excludeLayers = LayerMask.GetMask("Player","Viewmodel","InvisibleCollider", "Hitbox"); // still scuffed
			rb.AddForce(player.Camera.forward * 10f, ForceMode.VelocityChange);
			
			_ = capy.GameObject.AddComponent<NetworkRigidbodyUnreliable>(); 
			capy.GameObject.AddComponent<SelfDestructProjectile>().DelayedDestroy(5f);
		}
	}
}
