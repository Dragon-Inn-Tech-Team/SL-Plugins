using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomCommands.Features.CustomWeapons.Weapons
{
	public class RagdollLauncher : CustomWeaponBase
	{
		public override CustomWeaponsManager.CustomWeaponType WeaponType => CustomWeaponsManager.CustomWeaponType.Ragdoll;

		public override string Name => "Ragdoll Launcher";

		public override void ShootWeapon(Player player)
		{
			var role = CustomWeaponsManager.RagdollRoles[Random.Range(0, CustomWeaponsManager.RagdollRoles.Length - 1)];

			PlayerRoleLoader.TryGetRoleTemplate(role, out FpcStandardRoleBase pRB);
			var firearm = player.CurrentItem.Base as Firearm;

			var dh = new FirearmDamageHandler(firearm, 10, 10);

			Vector3 velocity = Vector3.zero;
			velocity += player.Camera.transform.forward * Random.Range(5f, 10f);
			velocity += player.Camera.transform.up * Random.Range(0.75f, 4.5f);

			if (Random.Range(1, 3) % 2 == 0)
				velocity += player.Camera.transform.right * Random.Range(0.1f, 2.5f);

			else
				velocity += player.Camera.transform.right * -Random.Range(0.1f, 2.5f);

			typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(dh, velocity);

			RagdollData data = new RagdollData(null, dh, role, new RelativePositioning.RelativePosition(player.Position), player.GameObject.transform.rotation, player.Nickname, NetworkTime.time);
			BasicRagdoll basicRagdoll = UnityEngine.Object.Instantiate(pRB.Ragdoll);
			basicRagdoll.NetworkInfo = data;
			NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
		}
	}
}
