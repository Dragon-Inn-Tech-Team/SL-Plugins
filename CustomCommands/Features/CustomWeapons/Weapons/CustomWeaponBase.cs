using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomCommands.Features.CustomWeapons.CustomWeaponsManager;

namespace CustomCommands.Features.CustomWeapons.Weapons
{
	public abstract class CustomWeaponBase
	{
		public abstract CustomWeaponType WeaponType { get; }
		public abstract string Name { get; }

		public virtual void EnableWeapon(Player player)
		{
			EnabledWeapons.AddToOrReplaceValue(player.UserId, WeaponType);
		}

		public virtual void DisableWeapon(Player player)
		{
			EnabledWeapons.Remove(player.UserId);
		}

		/// <summary>
		/// Toggles the weapon on or off for the player
		/// </summary>
		/// <param name="player"></param>
		/// <returns>True if the weapon has been toggled on for the player</returns>
		public virtual bool ToggleWeapon(Player player)
		{
			if (EnabledWeapons.ContainsKey(player.UserId))
			{
				DisableWeapon(player);
				return false;
			}
			else
			{
				EnableWeapon(player);
				return true;
			}				
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="player"></param>
		public virtual void ShootWeapon(Player player) { }

		public virtual void HitPlayer(PlayerHurtingEventArgs ev) { }

		public virtual void PlaceBulletHole(PlayerPlacedBulletHoleEventArgs ev) { }
	}
}
