﻿using CustomPlayerEffects;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RedRightHand
{
	public static class Helpers
	{
		public static Vector3 RandomThrowableVelocity(Transform Transform)
		{
			Vector3 velocity = Vector3.zero;
			velocity += Transform.forward * Random.Range(10f, 15f);
			velocity += Transform.up * 1f;

			if (Random.Range(1, 3) % 2 == 0)
				velocity += Transform.right * Random.Range(0.1f, 2.5f);

			else
				velocity += Transform.right * -Random.Range(0.1f, 2.5f);

			return velocity;
		}

		public static void SpawnGrenade<T>(Player Thrower, ItemType Item) where T : TimeGrenade =>
	SpawnGrenade<T>(Thrower, Item, new Vector3(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1)));

		public static void SpawnGrenade<T>(Player Thrower, ItemType Item, Vector3 Direction) where T : TimeGrenade
		{
			InventorySystem.Items.ThrowableProjectiles.ThrowableItem item = (InventorySystem.Items.ThrowableProjectiles.ThrowableItem)Thrower.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(Item, ItemSerialGenerator.GenerateNext()), false);
			Vector3 Pos = Thrower.Position;
			Pos.y += 1;

			T grenade = (T)UnityEngine.Object.Instantiate(item.Projectile, Pos, Quaternion.identity);
			grenade.NetworkInfo = new InventorySystem.Items.Pickups.PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
			grenade.Position = Pos;
			grenade.Rotation = Quaternion.identity;
			grenade.GetComponent<Rigidbody>().velocity = Direction;

			grenade.PreviousOwner = new Footprinting.Footprint(Thrower.ReferenceHub);
			Mirror.NetworkServer.Spawn(grenade.gameObject);
			grenade.ServerActivate();
		}

		public static BasicRagdoll SpawnRagdoll(RagdollData ragdollData)
		{
			PlayerRoleLoader.TryGetRoleTemplate(ragdollData.RoleType, out FpcStandardRoleBase ragdollRole);

			BasicRagdoll basicRagdoll = UnityEngine.Object.Instantiate<BasicRagdoll>(ragdollRole.Ragdoll);
			basicRagdoll.NetworkInfo = ragdollData;
			basicRagdoll.gameObject.AddComponent<FakeRagdoll>();
			NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
			return basicRagdoll;
		}

		public static BasicRagdoll SpawnRagdoll(string nickname, RoleTypeId role, Vector3 position, Quaternion rotation, Vector3 velocity, string deathReason)
		{
			PlayerRoleLoader.TryGetRoleTemplate(role, out FpcStandardRoleBase _);

			var dh = new CustomReasonDamageHandler(deathReason);

			typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(dh, velocity);

			return SpawnRagdoll(new RagdollData(null, dh, role, position, rotation, nickname, NetworkTime.time));
		}

		public static void RagdollPlayer(this Player plr, float time = 3, float forceMultiplyer = 1, bool teleportOnEnd = true, string ragdollText = "guh")
		{
			if (!plr.IsAlive)
				return;
			Vector3 velocity = plr.Velocity;
			velocity += plr.Camera.transform.forward * UnityEngine.Random.Range(1, 1.5f) * forceMultiplyer;

			velocity += plr.Camera.transform.up * UnityEngine.Random.Range(0.75f, 1.25f) * forceMultiplyer;
			var basicRagdoll = SpawnRagdoll(plr.Nickname, plr.Role, plr.Position, plr.Camera.rotation, velocity, ragdollText);

			var items = plr.ReferenceHub.inventory.UserInventory.Items;
			plr.CurrentItem = null;
			plr.ReferenceHub.inventory.UserInventory.Items = [];
			plr.EnableEffect<Invisible>(1, time, false);
			plr.EnableEffect<Ensnared>(1, time, false);

			MEC.Timing.CallDelayed(time, () =>
			{
				plr.ReferenceHub.inventory.UserInventory.Items = items;

				if (teleportOnEnd)
					plr.Position = basicRagdoll.CenterPoint.position + Vector3.up;

				NetworkServer.Destroy(basicRagdoll.gameObject);
			});
		}
	}
}
