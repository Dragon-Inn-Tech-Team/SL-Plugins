using CustomPlayerEffects;
using MapGeneration;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using Utils;

namespace CustomCommands.Features.Events.WeeklyEvents
{
	public class Events
	{
		[PluginEvent]
		public void OnCoinFlip(PlayerCoinFlipEvent ev)
		{
			if (ev.Player.IsHuman && ev.Player.Role != PlayerRoles.RoleTypeId.Tutorial)
			{
				if (EventManager.CurrentEvent == EventType.CoinFlipDeath)
				{
					if (ev.IsTails)
					{
						MEC.Timing.CallDelayed(2, () =>
						{
							ev.Player.RemoveItems(ItemType.Coin);
							ev.Player.CurrentItem = null;
							ExplosionUtils.ServerSpawnEffect(ev.Player.Position, ItemType.GrenadeHE);

						});
					}
				}

				//if (EventManager.CurrentEvent == EventType.CoinCardUpgrade)
				//{
				//	MEC.Timing.CallDelayed(2, () =>
				//	{
				//		ev.Player.RemoveItem(ev.Player.CurrentItem);
				//		ev.Player.CurrentItem = null;

				//		var itemHash = HashSetPool<ushort>.Shared.Rent();

				//		for (int i = 0; i < ev.Player.Items.Count; i++)
				//		{
				//			var item = ev.Player.Items.ElementAt(i);

				//			if (item.Category == ItemCategory.Keycard && item.TryGetComponent<Scp914ItemProcessor>(out Scp914ItemProcessor processor))
				//			{
				//				processor.OnInventoryItemUpgraded(!ev.IsTails ? Scp914KnobSetting.Fine : Scp914KnobSetting.Coarse, ev.Player.ReferenceHub, item.ItemSerial);
				//			}
				//		}

				//		ev.Player.ReceiveHint($"Your keycards have been {(!ev.IsTails ? "upgraded" : "downgraded")}", 5);
				//	});
				//}
			}
		}

		[PluginEvent]
		public void OnFlashlight(PlayerToggleFlashlightEvent ev)
		{
			if (EventManager.CurrentEvent == EventType.FlashlightDisco && ev.Player.Zone != MapGeneration.FacilityZone.Surface && ev.Player.CurrentItem.ItemTypeId == ItemType.Flashlight)
			{
				if (ev.IsToggled)
				{
					var color = new UnityEngine.Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f));
					var roomid = RoomIdUtils.RoomAtPosition(ev.Player.Position);
					if (roomid == null)
						return;

					var rLC = roomid.GetComponentInChildren<RoomLightController>();
					if (rLC == null)
						return;

					rLC.NetworkOverrideColor = color;
				}
			}
		}

		[PluginEvent]
		public void OnPlayerDisarm(PlayerHandcuffEvent ev)
		{
			if (EventManager.CurrentEvent == EventType.SpeedyDisarm)
			{
				ev.Target.EffectsManager.EnableEffect<MovementBoost>(1).Intensity = 20;
				ev.Target.ReceiveHint($"ZOOM ZOOM", 2);
			}
		}

		[PluginEvent]
		public void OnPlayerDamage(PlayerDamageEvent ev)
		{
			if (ev.DamageHandler is AttackerDamageHandler aDH && ev.Target != null)
			{
				if (EventManager.CurrentEvent == EventType.HealthStealer15 && ev.Player.CurrentItem.ItemTypeId == ItemType.GunCOM15 && ev.Target.IsHuman && !ev.Target.IsGodModeEnabled)
				{
					ev.Player.Heal(aDH.Damage);
				}
			}
		}

		[PluginEvent]
		public bool OnGunReload(PlayerReloadWeaponEvent ev)
		{
			if (EventManager.CurrentEvent == EventType.HealthStealer15)
			{
				if (ev.Firearm.ItemTypeId == ItemType.GunCOM15)
				{
					ev.Player.ReceiveHint($"You cannot reload this gun", 3);
					return false;
				}
			}
			return true;
		}
	}
}
