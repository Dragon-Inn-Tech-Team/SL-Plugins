using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using RedRightHand.Core;
using System.Linq;

namespace CustomCommands.Features.Humans.Disarming
{
	public class DisarmingEvents
	{
		[PluginEvent]
		public void PlayerDisarmed(PlayerHandcuffEvent args)
		{
			if (args.Target.Role == RoleTypeId.ClassD && !args.Target.TemporaryData.Contains("kosdisarm"))
			{
				args.Target.TemporaryData.StoredData.Add("kosdisarm", (int)1);
			}
		}

		[PluginEvent]
		public void PlayerDamaged(PlayerDamageEvent args)
		{
			if (args.DamageHandler is FirearmDamageHandler fDH)
			{
				var isVicClassD = args.Target.Role == RoleTypeId.ClassD;
				var isAtkrFacGuard = args.Player.Role == RoleTypeId.FacilityGuard;
				var hasVicDisarmed = !args.Target.TemporaryData.Contains("kosdisarm");
				var hasExclusionItems = !args.Target.ReferenceHub.inventory.UserInventory.Items.Where(i =>
					i.Value.Category == ItemCategory.Firearm ||
					i.Value.Category == ItemCategory.SpecialWeapon ||
					(i.Value.Category == ItemCategory.SCPItem && i.Value.ItemTypeId != ItemType.SCP330) ||
					i.Value.Category == ItemCategory.Grenade).Any();

				if (isVicClassD && isAtkrFacGuard && hasVicDisarmed && hasExclusionItems)
				{
					fDH.UpdatePrivateProperty("Damage", fDH.Damage / 2);
				}
			}
		}
	}
}
