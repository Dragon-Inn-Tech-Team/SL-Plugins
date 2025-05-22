using InventorySystem.Items.Pickups;
using MapGeneration;
using MapGeneration.Distributors;
using Mirror;
using PlayerRoles.PlayableScps.Scp079;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCommands.Events
{
	public class SCP008
	{
		[PluginEvent(ServerEventType.MapGenerated)]
		public void MapGenEvent(MapGeneratedEvent args)
		{
			var room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Hcz939).First();
			var prefab = NetworkClient.prefabs.Where(r => r.Value.name == "Scp1853PedestalStructure Variant").First();

			var pedestal = GameObject.Instantiate(prefab.Value);
			pedestal.transform.parent = room.transform;

			pedestal.transform.localPosition = new UnityEngine.Vector3(3.8f, 0, 5.8f);

			pedestal.transform.rotation = room.transform.rotation;
			pedestal.transform.Rotate(0, -90, 0);

			NetworkServer.Spawn(pedestal);

			pedestal.AddComponent<SCP008Pedestal>();
			pedestal.AddComponent<Scp079Speaker>();
		}

		[PluginEvent(ServerEventType.RoundStart)]
		public void RoundRestart(RoundStartEvent args)
		{
			foreach (var a in UnityEngine.Object.FindObjectsOfType<PedestalScpLocker>())
			{
				if (a.GetComponent<SCP008Pedestal>() != null)
				{
					var chamber = a.GetComponent<PedestalScpLocker>().Chambers.First();

					var privProp = chamber.GetType().GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic);
					var content = (HashSet<ItemPickupBase>)privProp.GetValue(chamber);

					foreach (ItemPickupBase c in content)
					{
						c.gameObject.AddComponent<SCP008Item>();
					}
				}
			}
		}

		[PluginEvent(ServerEventType.PlayerUsedItem)]
		public void ItemUsed(PlayerUsedItemEvent args)
		{
			if (args.Item.ItemTypeId == ItemType.SCP1853)
			{
				if (args.Item.gameObject.TryGetComponent<SCP008Item>(out _))
				{
					if (!args.Player.TemporaryData.Contains("scp008infection"))
					{
						args.Player.ReceiveHint("You were not infected");
					}
				}

				Log.Info(args.Item.gameObject.tag);
			}
		}
	}

	public class SCP008Pedestal : MonoBehaviour
	{
		public bool PendingInfection = true;
	}

	public class SCP008Item : MonoBehaviour
	{

	}
}
