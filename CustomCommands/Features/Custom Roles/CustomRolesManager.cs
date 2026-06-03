using CustomCommands.Core;
using CustomCommands.Core.Roles;
using CustomCommands.Features.Custom.Roles.Roles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using System.Collections.Generic;

namespace CustomCommands.Features.Custom.Roles
{
	public class CustomRolesManager : CustomFeatureBase
	{
		//Ideas
		//Medic: Has extra medical items, and can heal other players more efficiently.
		//Tank: Has a size of 1.2, has a max HP of 120, and heavy armour, but moves slightly slower.
		//Scout: A smaller size of 0.85, has a max HP of 85, no armour, a mp7, but move at 2x cola speed.
		//Hacker: Has a custom card that can disable tesla gates for 8 seconds, and can open any door locked by 079/blackouts

		public enum CustomRoleType
		{
			NONE, Medic, Tank, Scout, Hacker
		}

		public static Dictionary<string, CustomRoleType> ActiveRoles = new Dictionary<string, CustomRoleType>();
		public static Dictionary<CustomRoleType, CustomRoleBase> AvailableRoles = new Dictionary<CustomRoleType, CustomRoleBase>();

		public CustomRolesManager(bool configSetting) : base(configSetting)
		{
			if (IsEnabled)
			{
				AvailableRoles = new Dictionary<CustomRoleType, CustomRoleBase>
				{
					{CustomRoleType.Medic, new MedicRole() },
					{CustomRoleType.Tank, new TankRole() },
				};
			}
		}

		public static void EnableRole(Player player, CustomRoleType roleType)
		{
			if (ActiveRoles.ContainsKey(player.UserId))
				DisableRole(player);

			if (AvailableRoles.ContainsKey(roleType))
			{
				AvailableRoles[roleType].EnableRole(player);
			}
		}

		public static void DisableRole(Player player)
		{
			if (ActiveRoles.ContainsKey(player.UserId))
			{
				AvailableRoles[ActiveRoles[player.UserId]].DisableRole(player);
			}
		}

		public override void OnServerWaitingForPlayers()
		{
			ActiveRoles.Clear();
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (ActiveRoles.TryGetValue(ev.Player.UserId, out CustomRoleType type) && AvailableRoles.TryGetValue(type, out var cusRole))
				cusRole.PlayerHurt(ev);
		}

		public override void OnPlayerDeath(PlayerDeathEventArgs ev)
		{
			if (ActiveRoles.TryGetValue(ev.Player.UserId, out CustomRoleType type) && AvailableRoles.TryGetValue(type, out var cusRole))
				cusRole.PlayerDied(ev);
		}

		public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
		{
			if (ActiveRoles.TryGetValue(ev.Player.UserId, out CustomRoleType type) && AvailableRoles.TryGetValue(type, out var cusRole))
				cusRole.PlayerChangeRole(ev);
		}
	}
}
