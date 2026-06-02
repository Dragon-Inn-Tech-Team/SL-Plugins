using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerRoles;
using PlayerStatsSystem;
using RedRightHand;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.Disarming
{
	public class BetterDisarming : CustomFeatureBase
	{
		List<string> disarmedUsers = new List<string>();
		Dictionary<string, int> kosDict = new Dictionary<string, int>();

		public BetterDisarming(bool configSetting) : base(configSetting)
		{
		}

		public override void OnPlayerCuffed(PlayerCuffedEventArgs ev)
		{
			if (ev.Target.Role == RoleTypeId.ClassD && !disarmedUsers.Contains(ev.Target.UserId))
				disarmedUsers.Add(ev.Target.UserId);
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (ev.DamageHandler is FirearmDamageHandler fDH)
			{
				var isVicClassD = ev.Player.Role == RoleTypeId.ClassD;
				var isAtkrFacGuard = ev.Attacker.Role == RoleTypeId.FacilityGuard;
				var hasVicDisarmed = !disarmedUsers.Contains(ev.Player.UserId);
				var hasExclusionItems = !ev.Player.ReferenceHub.inventory.UserInventory.Items.Where(i =>
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

		public override void OnServerWaitingForPlayers()
		{
			kosDict.Clear();
			disarmedUsers.Clear();
		}
	}
}
