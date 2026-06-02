using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using Mirror;
using RedRightHand;
using UnityEngine;
using static CustomCommands.Features.Custom.Weapons.CustomWeaponsManager;

namespace CustomCommands.Core
{
	public abstract class CustomWeaponBase
	{
		public abstract ItemType ItemType { get; }
		public abstract string Name { get; }

		public virtual void EnableWeapon(Item firearm)
		{
			EnabledWeapons.AddToOrReplaceValue(firearm.Serial, this);
		}

		/// <summary>
		/// Returns true if the weapon has been toggled on for the player
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public virtual void GiveWeapon(Player player)
		{
			var weapon = player.AddItem(ItemType);
			EnableWeapon(weapon);
		}

		public virtual void ShootWeapon(Player player) { }

		public virtual void HitPlayer(PlayerHurtingEventArgs ev) { }

		public virtual void PlaceBulletHole(PlayerPlacedBulletHoleEventArgs ev) { }

		public virtual void ReloadingWeapon(PlayerReloadingWeaponEventArgs ev) { }

		public virtual void WeaponPickedUp(PlayerSearchedPickupEventArgs ev)
		{
			ev.Player.SendHint($"You have picked up a {Name}", 3);
		}

		public virtual void WeaponEquipped(PlayerChangedItemEventArgs ev)
		{
			ev.Player.SendHint($"You have equipped a {Name}");
		}

		/// <summary>
		/// Component for cleaning up custom projectiles more easily
		/// </summary>
		public class SelfDestructProjectile : MonoBehaviour
		{
			public void DelayedDestroy(float delay)
			{
				MEC.Timing.CallDelayed(delay, () => { NetworkServer.Destroy(gameObject); });
			}
		}
	}
}
