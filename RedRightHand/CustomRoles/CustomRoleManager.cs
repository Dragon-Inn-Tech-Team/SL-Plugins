using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MonoMod.Utils;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHand.CustomRoles
{
	public static class CustomRolesManager
	{
		static CustomEventsHandler eventHandler;

		//Ideas
		//Medic: Has extra medical items, and can heal other players more efficiently.
		//Tank: Has a size of 1.2, has a max HP of 120, and heavy armour, but moves slightly slower.
		//Scout: A smaller size of 0.85, has a max HP of 85, no armour, a mp7, but move at 2x cola speed.
		//Hacker: Has a custom card that can disable tesla gates for 8 seconds, and can open any door locked by 079/blackouts

		static Dictionary<string, string> ActiveRoles = [];
		public static Dictionary<string, CustomRoleBase> AvailableRoles = [];

		/// <summary>
		/// Register a custom role to be available for plugins
		/// </summary>
		/// <param name="roles"></param>
		public static void RegisterRoles(Dictionary<string, CustomRoleBase> roles)
		{
			AvailableRoles.AddRange(roles);
		}
		/// <summary>
		/// Unregister a custom role, marking it as unavailable.<br/>This should be avoided where possible.
		/// </summary>
		/// <param name="roles"></param>
		public static void UnregisterRoles(string[] roles)
		{
			foreach (var r in roles)
			{
				if (AvailableRoles.ContainsKey(r))
					AvailableRoles.Remove(r);
			}
		}

		/// <summary>
		/// Enable a custom role for a player. It will automatically disable any other currently active custom role for the specified player.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="roleType"></param>
		public static void EnableRole(Player player, string roleType)
		{
			if (ActiveRoles.ContainsKey(player.UserId))
				DisableRole(player);

			if (AvailableRoles.ContainsKey(roleType))
			{
				AvailableRoles[roleType].EnableRole(player);
				ActiveRoles.Add(player.UserId, roleType);
			}
		}
		/// <summary>
		/// Disable any currently active custom role for the specified player
		/// </summary>
		/// <param name="player"></param>
		public static void DisableRole(Player player, bool removeFromDict = true)
		{
			if (ActiveRoles.ContainsKey(player.UserId))
			{
				AvailableRoles[ActiveRoles[player.UserId]].DisableRole(player);

				if (removeFromDict)
					ActiveRoles.Remove(player.UserId);
			}
		}


		/// <summary>
		/// Disables all currently active custom roles from all players
		/// </summary>
		public static void DisableAllRoles(bool skipOnlineCheck = false)
		{
			if (!skipOnlineCheck)
			{
				foreach (var kvp in ActiveRoles)
				{
					if (Player.TryGet(kvp.Key, out var plr))
						DisableRole(plr, false);
				}
			}

			ActiveRoles.Clear();
		}

		/// <summary>
		/// Returns if the player has a currently active role, along with the <see cref="CustomRoleType">Type</see> and <see cref="CustomRoleBase">Base</see>
		/// </summary>
		/// <param name="player"></param>
		/// <param name="roleType"></param>
		/// <param name="roleBase"></param>
		/// <returns>Returns true if the player is currently playing as a custom role</returns>
		public static bool GetCustomRole(this Player player, out CustomRoleBase roleBase)
		{
			if (!ActiveRoles.ContainsKey(player.UserId))
			{
				roleBase = null;
				return false;
			}

			else
			{
				roleBase = AvailableRoles[ActiveRoles[player.UserId]];
				return true;
			}
		}

		public static void RegisterEvents()
		{
			if (eventHandler == null)
				CustomHandlersManager.RegisterEventsHandler(new CustomRolesEventsManager());
		}
	}

	public class CustomRolesEventsManager : CustomEventsHandler
	{
		public override void OnServerWaitingForPlayers()
		{
			CustomRolesManager.DisableAllRoles(true);
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (ev.Player.GetCustomRole(out var customRole))
				customRole.PlayerHurt(ev);
		}

		public override void OnPlayerDeath(PlayerDeathEventArgs ev)
		{
			if (ev.Player.GetCustomRole(out var customRole))
				customRole.PlayerDied(ev);
		}

		public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
		{
			if (ev.Player.GetCustomRole(out var customRole))
				customRole.PlayerChangeRole(ev);
		}
	}
}
