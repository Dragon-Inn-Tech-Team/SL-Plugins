using CommandSystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using MapGeneration;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Features.Events.DClassBattle
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class BattleCommand : ICustomCommand
	{
		public static ItemType[] items = new ItemType[]
		{
			ItemType.MicroHID,
			ItemType.GunRevolver,
			ItemType.Jailbird,
			ItemType.ParticleDisruptor,
		};

		public string Command => "battle";

		public string[] Aliases => null;

		public string Description => "Spawns everyone as a D-class, teleports them to 914, and gives them a random weapon";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.events";

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var plrs, out var psender))
				return false;


			Plugin.CurrentEvent = EventType.Battle;
			Round.IsLocked = true;

			var room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Lcz914);
			var room914 = room.First();

			ItemType item = items[UnityEngine.Random.Range(0, items.Length - 1)];

			foreach (Player plr in Player.GetPlayers())
			{
				if (plr.IsServer || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				plr.SetRole(PlayerRoles.RoleTypeId.ClassD, PlayerRoles.RoleChangeReason.RemoteAdmin);
				plr.Position = new Vector3(room914.ApiRoom.Position.x, room914.ApiRoom.Position.y + 1, room914.ApiRoom.Position.z);

				plr.ClearInventory();
				var itemBase = plr.AddItem(item);

				plr.ReferenceHub.inventory.ServerSelectItem(itemBase.ItemSerial);

				if (item == ItemType.GunRevolver)
				{
					var firearm = itemBase as Firearm;

					AttachmentsUtils.ApplyAttachmentsCode(firearm, 1170, true);
					if (firearm.TryGetModule<IPrimaryAmmoContainerModule>(out var iPACM))
					{
						iPACM.ServerModifyAmmo(firearm.GetTotalMaxAmmo());
					}
				}

				plr.ReceiveHint("Kill them all!!!");
			}

			response = $"D-class battle has begun";
			return true;
		}
	}
}
