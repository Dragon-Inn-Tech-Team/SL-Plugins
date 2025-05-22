using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using PluginAPI.Core;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CustomCommands.Features.Items
{
	public static class ItemManager
	{
		public static ItemPickupBase TranqGun;
		public static bool TranqGunSet
		{
			get
			{
				return Plugin.Config.EnableTranqGun && TranqGun != null && TranqGun.Info.Serial != 0;
			}
		}

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
			ThrowableItem item = (ThrowableItem)Thrower.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(Item, ItemSerialGenerator.GenerateNext()), false);
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

		public static ItemPickupBase[] GetItemsOfType(ItemType Type)
		{
			List<ItemPickupBase> items = new List<ItemPickupBase>();
			foreach (var item in Object.FindObjectsOfType<ItemPickupBase>())
			{
				if (item.Info.ItemId == Type)
					items.Add(item);
			}

			return items.ToArray();
		}
	}
}
