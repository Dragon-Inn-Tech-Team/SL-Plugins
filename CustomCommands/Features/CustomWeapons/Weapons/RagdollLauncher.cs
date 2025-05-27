using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using RedRightHand;
using RedRightHand.CustomWeapons;
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
		public override string Name => "Ragdoll Launcher";
		public override ItemType Model => ItemType.GunCom45;

		static RoleTypeId[] ragdolls = new RoleTypeId[]
		{
					RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRepressor,
					RoleTypeId.NtfCaptain, RoleTypeId.NtfSpecialist, RoleTypeId.NtfPrivate, RoleTypeId.NtfSergeant, RoleTypeId.Tutorial
		};

		public override void ShootWeapon(PlayerShootingWeaponEventArgs ev)
		{
			var role = ragdolls[Random.Range(0, ragdolls.Length - 1)];

			PlayerRoleLoader.TryGetRoleTemplate(role, out FpcStandardRoleBase pRB);
			var firearm = ev.Player.CurrentItem.Base as Firearm;

			var dh = new FirearmDamageHandler(firearm, 10, 10);

			Vector3 velocity = Vector3.zero;
			velocity += ev.Player.Camera.transform.forward * Random.Range(5f, 10f);
			velocity += ev.Player.Camera.transform.up * Random.Range(0.75f, 4.5f);

			if (Random.Range(1, 3) % 2 == 0)
				velocity += ev.Player.Camera.transform.right * Random.Range(0.1f, 2.5f);

			else
				velocity += ev.Player.Camera.transform.right * -Random.Range(0.1f, 2.5f);

			typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(dh, velocity);

			RagdollData data = new RagdollData(null, dh, role, ev.Player.Position, ev.Player.GameObject.transform.rotation, ev.Player.Nickname, NetworkTime.time);
			BasicRagdoll basicRagdoll = UnityEngine.Object.Instantiate(pRB.Ragdoll);
			basicRagdoll.NetworkInfo = data;
			NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
		}
	}
}
