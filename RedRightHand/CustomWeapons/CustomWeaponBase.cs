using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using RedRightHand;

namespace RedRightHand.CustomWeapons
{
	public abstract class CustomWeaponBase
	{
		public uint Serial { get; private set; }
		public abstract string Name { get; }
		public abstract ItemType Model { get; }

		public virtual void EnableWeapon(uint serial)
		{
			this.Serial = serial;
		}

		public virtual void DisableWeapon()
		{

		}

		public virtual void ShootWeapon(PlayerShootingWeaponEventArgs ev) { }

		public virtual void HitPlayer(PlayerHurtingEventArgs ev) { }

		public virtual void PlaceBulletHole(PlayerPlacedBulletHoleEventArgs ev) { }
	}
}
