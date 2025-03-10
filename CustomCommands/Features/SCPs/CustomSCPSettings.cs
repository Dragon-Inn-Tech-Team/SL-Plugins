using CustomCommands.Features.SCPs.Swap;
using RedRightHand.Core.CustomSettings;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace CustomCommands.Features.SCPs
{
	public class CustomSCPSettings : CustomSettingsBase
	{
		/// <summary>
		/// Name of the setting. This is used in the header for the settings menu.
		/// </summary>
		public override string Name => "SCP Settings";
		/// <summary>
		/// Quick description of what the settings do.
		/// </summary>
		public override string Description => "Various settings for our SCP swap system";
		/// <summary>
		/// Array of <see cref="ServerSpecificSettingBase"/> settings to appear on the client.
		/// This must always be started with a <see cref="SSGroupHeader"/> option containing the name and description.
		/// </summary>
		public override ServerSpecificSettingBase[] SettingBases => new ServerSpecificSettingBase[]
		{
			//Basic header for this section of the settings list. This will be used to give a quick description of what all the settings below are used for.
			new SSGroupHeader(Name, false, Description),
			//Custom keybind option. Default bind set to O (not Zero), blocks input while GUIs are open, and adds a simple descriptive hint for the option.
			new SSKeybindSetting((int) SettingsIDs.SCP_SwapToHuman, "Swap to Human", KeyCode.O, true, "Swap from an SCP to a Human role."),
			new SSKeybindSetting((int) SettingsIDs.SCP_SwapFromHuman, "Swap to SCP", KeyCode.L, true, "Replaces an SCP if an SCP slot is available."),
			//Custom setting with 2 toggle buttons. The 2nd option (Referred to as B in all code) is set as the default option for users. Also has a simple descriptive hint for the option.
			new SSTwoButtonsSetting((int) SettingsIDs.SCP_NeverSCP, "Auto swap from SCP", "Enabled", "Disabled", true, "Always get swapped to a human role if you spawn as an SCP (And are not on cooldown from doing so)")
		};

		/// <summary>
		/// Method used to register events used by the settings. You will mainly be using <see cref="ServerSpecificSettingsSync.ServerOnSettingValueReceived"/>.
		/// </summary>
		public override void Activate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived += this.ProcessUserInput;
		}

		/// <summary>
		/// Opposite of <see cref="CustomSCPSettings.Activate"/>. Used to unregister events when the settings are disposed.
		/// </summary>
		public override void Deactivate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived -= this.ProcessUserInput;
		}

		/// <summary>
		/// Used to process inputs (both when a keybind is pressed, and when a setting is changed) from users.
		/// </summary>
		/// <param name="hub"></param>
		/// <param name="setting"></param>
		public override void ProcessUserInput(ReferenceHub hub, ServerSpecificSettingBase setting)
		{
			//Simple switch statement for the setting ID.
			//All setting IDs are ints, and each setting has a unique ID given by us when the setting is registered.
			switch (setting.SettingId)
			{
				case (int)SettingsIDs.SCP_SwapToHuman:
					{
						//To get the data we need, we need to first cast the setting into it's appropriate SSSB (In this case, a KeybindSetting). We then need to check if the synced key is being pressed.
						//If the key is being pressed, run whatever it is we need to run. In this case, it will swap the SCP to a human role.
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed)
						{
							if (SwapManager.CanScpSwapToHuman(hub, out string reason))
								SwapManager.SwapScpToHuman(hub);
							else
								hub.gameConsoleTransmission.SendToClient(reason, "green");
						}
						break;
					}

				case (int)SettingsIDs.SCP_SwapFromHuman:
					{
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed)
						{
							if (SwapManager.CanHumanSwapToScp(hub, true, out string reason))
								SwapManager.QueueSwapHumanToScp(hub);
							else
								hub.gameConsoleTransmission.SendToClient(reason, "green");
						}

						break;
					}

				default:
					break;
			}
		}
	}
}
