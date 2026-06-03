using CustomCommands.Core.Roles;
using LabApi.Features.Wrappers;

namespace CustomCommands.Features.Custom.Roles.Roles
{
	public class MedicRole : CustomRoleBase
	{
		public override CustomRolesManager.CustomRoleType CustomRole => CustomRolesManager.CustomRoleType.Medic;

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
