using CustomPlayerEffects;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CustomCommands.Features.Ragdoll
{
	public static class PlayerRagdollManager
	{
		public static BasicRagdoll SpawnRagdoll(RagdollData ragdollData, StandardDamageHandler dh)
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
			PlayerRoleLoader.TryGetRoleTemplate(role, out FpcStandardRoleBase ragdollRole);

			var dh = new CustomReasonDamageHandler(deathReason);

			typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(dh, velocity);

			return SpawnRagdoll(new RagdollData(null, dh, role, position, rotation, nickname, NetworkTime.time), dh);
		}

		public static void RagdollPlayer(this Player plr, float time = 3, float forceMultiplyer = 1, bool teleportOnEnd = true)
		{
			if (!plr.IsAlive)
				return;
			Vector3 velocity = plr.Velocity;
			velocity += plr.Camera.transform.forward * UnityEngine.Random.Range(1, 1.5f) * forceMultiplyer;

			velocity += plr.Camera.transform.up * UnityEngine.Random.Range(0.75f, 1.25f) * forceMultiplyer;
			var basicRagdoll = SpawnRagdoll(plr.Nickname, plr.Role, plr.Position, plr.Camera.rotation, velocity, "guh");

			var items = plr.ReferenceHub.inventory.UserInventory.Items;
			plr.CurrentItem = null;
			plr.ReferenceHub.inventory.UserInventory.Items = new Dictionary<ushort, InventorySystem.Items.ItemBase>();
			plr.EffectsManager.EnableEffect<Invisible>(time);
			plr.EffectsManager.EnableEffect<Ensnared>(time);

			MEC.Timing.CallDelayed(time, () =>
			{
				plr.ReferenceHub.inventory.UserInventory.Items = items;

				if (teleportOnEnd)
					plr.Position = basicRagdoll.CenterPoint.position + Vector3.up;

				NetworkServer.Destroy(basicRagdoll.gameObject);
			});
		}


		public static void RagdollPlayerTranqGun(this Player plr, Player cause, float time = 3)
		{
			Vector3 velocity = plr.Velocity;
			var basicRagdoll = SpawnRagdoll(plr.Nickname, plr.Role, plr.Position, plr.GameObject.transform.rotation, velocity, "guh");

			var items = plr.ReferenceHub.inventory.UserInventory.Items;
			plr.CurrentItem = null;
			plr.ReferenceHub.inventory.UserInventory.Items = new Dictionary<ushort, InventorySystem.Items.ItemBase>();
			plr.EffectsManager.EnableEffect<Invisible>(time);
			plr.EffectsManager.EnableEffect<Ensnared>(time);

			MEC.Timing.CallDelayed(time, () =>
			{

				plr.ReferenceHub.inventory.UserInventory.Items = items;
				plr.Position = basicRagdoll.CenterPoint.position + Vector3.up;
				NetworkServer.Destroy(basicRagdoll.gameObject);

			});
		}
	}
}
