using CustomPlayerEffects;
using InventorySystem;
using PlayerRoles;
using PlayerStatsSystem;
using RedRightHand.Core.CustomSettings;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace CustomCommands.Features.Humans
{
	public class CustomHumanSettings : CustomSettingsBase
	{
		public override string Name => "Human Settings";

		public override string Description => "Various settings for custom human controls";

		public override ServerSpecificSettingBase[] SettingBases => new ServerSpecificSettingBase[]
		{
			new SSGroupHeader(Name, false, Description),
			new SSKeybindSetting((int)SettingsIDs.Human_HealOther, "Heal Player", KeyCode.K, true, "Heals the player you are currently looking at (Requires a medkit)"),
			new SSKeybindSetting((int)SettingsIDs.Human_Suicide, "Suicide", KeyCode.KeypadEquals, true, "Kill yourself and become a spectator"),
		};

		public override void Activate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived += this.ProcessUserInput;
		}

		public override void Deactivate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived -= this.ProcessUserInput;
		}

		public override void ProcessUserInput(ReferenceHub hub, ServerSpecificSettingBase setting)
		{
			switch (setting.SettingId)
			{
				case (int)SettingsIDs.Human_Suicide:
					{
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed && hub.IsHuman())
						{
							hub.playerEffectsController.DisableEffect<SpawnProtected>();
							hub.playerStats.DealDamage(new CustomReasonDamageHandler("Sudden loss of brain function", 1000));
						}
						break;
					}

				case (int)SettingsIDs.Human_HealOther:
					{
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed)
						{
							if (hub.inventory.CurItem.TypeId == ItemType.Medkit)
							{
								if (Physics.Raycast(hub.PlayerCameraReference.position, hub.PlayerCameraReference.forward, out RaycastHit info, 3.5f))
								{
									if (info.collider.TryGetComponent(out HitboxIdentity hbI) && !HitboxIdentity.IsEnemy(hbI.TargetHub, hub) && hbI.TargetHub != hub)
									{
										hbI.TargetHub.playerStats.GetModule<HealthStat>().ServerHeal(40);
										hub.inventory.ServerRemoveItem(hub.inventory.CurItem.SerialNumber, null);
									}
								}
							}
						}

						break;
					}

				default:
					break;
			}
		}
	}
}
