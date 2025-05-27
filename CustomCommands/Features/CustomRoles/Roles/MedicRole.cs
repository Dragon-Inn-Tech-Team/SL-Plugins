using InventorySystem.Configs;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using RedRightHand.CustomRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomRoles.Roles
{
	public class MedicRole : CustomRoleBase
	{
		public override string Name => "Medic";

		public override void EnableRole(Player player)
		{
			base.EnableRole(player);

			player.ClearInventory(true, true);

			if (player.Team == PlayerRoles.Team.ChaosInsurgency)
				player.AddItem(ItemType.KeycardChaosInsurgency);
			else
				player.AddItem(ItemType.KeycardMTFOperative);

			player.AddItem(ItemType.GunCOM15);
			player.AddItem(ItemType.ArmorLight);
			player.AddItem(ItemType.Medkit);
			player.AddItem(ItemType.Medkit);
			player.AddItem(ItemType.Adrenaline);
			player.AddItem(ItemType.Radio);

			player.AddAmmo(ItemType.Ammo9x19, 120);
		}
	}
}
