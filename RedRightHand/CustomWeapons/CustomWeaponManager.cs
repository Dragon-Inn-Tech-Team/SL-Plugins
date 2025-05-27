using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MonoMod.Utils;
using PlayerStatsSystem;
using RedRightHand.CustomRoles;
using System.Collections.Generic;

namespace RedRightHand.CustomWeapons
{
	public static class CustomWeaponManager
	{
		static CustomEventsHandler eventHandler;
		static Dictionary<ushort, string> ActiveWeapons = [];
		public static Dictionary<string, CustomWeaponBase> AvailableWeapons = [];

		/// <summary>
		/// Register a custom weapon to be available for plugins
		/// </summary>
		/// <param name="weapons"></param>
		public static void RegisterWeapons(Dictionary<string, CustomWeaponBase> weapons)
		{
			AvailableWeapons.AddRange(weapons);
		}
		/// <summary>
		/// Unregister a custom weapon, marking it as unavailable.<br/>This should be avoided where possible.
		/// </summary>
		/// <param name="weapons"></param>
		public static void UnregisterWeapons(string[] weapons)
		{
			foreach (var w in weapons)
			{
				if (AvailableWeapons.ContainsKey(w))
					AvailableWeapons.Remove(w);
			}
		}

		/// <summary>
		/// Enable a custom weapon for the specified item serial. It will automatically disable any other currently active custom weapon for the specified serial.
		/// </summary>
		/// <param name="serial"></param>
		/// <param name="weaponType"></param>
		public static void EnableWeapon(ushort serial, string weaponType)
		{
			if (ActiveWeapons.ContainsKey(serial))
				DisableWeapon(serial);

			if (AvailableWeapons.ContainsKey(weaponType))
			{
				AvailableWeapons[weaponType].EnableWeapon(serial);
				ActiveWeapons.Add(serial, weaponType);
			}
		}
		/// <summary>
		/// Disable any currently active custom weapon for the specified serial
		/// </summary>
		/// <param name="serial"></param>
		public static void DisableWeapon(ushort serial, bool removeFromDict = true)
		{
			if (ActiveWeapons.ContainsKey(serial))
			{
				AvailableWeapons[ActiveWeapons[serial]].DisableWeapon();

				if (removeFromDict)
					ActiveWeapons.Remove(serial);
			}
		}


		/// <summary>
		/// Disables all currently active custom weapons from all serials
		/// </summary>
		public static void DisableAllWeapons(bool skipOnlineCheck = false)
		{
			if (!skipOnlineCheck)
			{
				foreach (var kvp in ActiveWeapons)
				{
					DisableWeapon(kvp.Key, false);
				}
			}

			ActiveWeapons.Clear();
		}

		/// <summary>
		/// Returns if the serial is currently a custom weapon, along with the <see cref="CustomRoleBase">Base</see>
		/// </summary>
		/// <param name="serial"></param>
		/// <param name="weaponBase"></param>
		/// <returns>Returns true if the specified serial is currently a custom weapon</returns>
		public static bool GetCustomWeapon(this ushort serial, out CustomWeaponBase weaponBase)
		{
			if (!ActiveWeapons.ContainsKey(serial))
			{
				weaponBase = null;
				return false;
			}

			else
			{
				weaponBase = AvailableWeapons[ActiveWeapons[serial]];
				return true;
			}
		}

		public static void RegisterEvents()
		{
			if (eventHandler == null)
				CustomHandlersManager.RegisterEventsHandler(new CustomRolesEventsManager());
		}
	}

	public class CustomWeaponsEventsManager : CustomEventsHandler
	{
		public override void OnServerWaitingForPlayers()
		{
			CustomWeaponManager.DisableAllWeapons();
		}

		public override void OnPlayerShootingWeapon(PlayerShootingWeaponEventArgs ev)
		{
			if (ev.FirearmItem.Serial.GetCustomWeapon(out var weaponBase))
				weaponBase.ShootWeapon(ev);
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (ev.DamageHandler is FirearmDamageHandler fADH && fADH.Firearm.ItemSerial.GetCustomWeapon(out var weaponBase))
				weaponBase.HitPlayer(ev);			
		}

		public override void OnPlayerPlacedBulletHole(PlayerPlacedBulletHoleEventArgs ev)
		{
			if(ev.Player.CurrentItem.Serial.GetCustomWeapon(out var weaponBase))
				weaponBase.PlaceBulletHole(ev);
		}
	}
}
