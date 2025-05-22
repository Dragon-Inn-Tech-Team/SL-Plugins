using CustomPlayerEffects;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapGeneration;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace Choas.SSSettings
{
    public class CustomGlobalSettings : CustomSettingsBase
    {
        /// <summary>
        /// Name of the setting. This is used in the header for the settings menu.
        /// </summary>
        public override string Name => "Global Settings";
        /// <summary>
        /// Quick description of what the settings do.
        /// </summary>
        public override string Description => "Global settings for the server";
        /// <summary>
        /// Array of <see cref="ServerSpecificSettingBase"/> settings to appear on the client.
        /// This must always be started with a <see cref="SSGroupHeader"/> option containing the name and description.
        /// </summary>
        public override ServerSpecificSettingBase[] SettingBases => new ServerSpecificSettingBase[]
        {
			//Basic header for this section of the settings list. This will be used to give a quick description of what all the settings below are used for.
			new SSGroupHeader(Name, false, Description),
			//Custom keybind option. Default bind set to O (not Zero), blocks input while GUIs are open, and adds a simple descriptive hint for the option.
			new SSKeybindSetting((int) CustomSettingsManager.SettingsIDs.SpecialButton, "Special button", KeyCode.E, false, "Special button that does something depending on your class."),
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

        public TimeSpan LastSpectatorTrigger = new TimeSpan(0);

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
                case (int)CustomSettingsManager.SettingsIDs.SpecialButton:
                    {
                        //To get the data we need, we need to first cast the setting into it's appropriate SSSB (In this case, a KeybindSetting). We then need to check if the synced key is being pressed.
                        //If the key is being pressed, run whatever it is we need to run.
                        if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed)
                        {
                            if (hub.roleManager.CurrentRole.RoleTypeId == PlayerRoles.RoleTypeId.Spectator)
                            {
                                if (Round.Duration.Seconds <= LastSpectatorTrigger.Seconds + 5)
                                {
                                    //hub.gameConsoleTransmission.SendToClient("On cooldown", "red");
                                    break;
                                }
                                var trgt = ReferenceHub.AllHubs.FirstOrDefault(x => x.IsSpectatedBy(hub));
                                if (trgt == default)
                                {
                                    hub.gameConsoleTransmission.SendToClient("Not spectating a player", "red");
                                    break;
                                }
                                if (UnityEngine.Random.Range(0f, 1f) > .75f)
                                {
                                    RoomIdUtils.RoomAtPosition(trgt.PlayerCameraReference.position).ApiRoom.Lights.FlickerLights(.1f);
                                    hub.gameConsoleTransmission.SendToClient("Lights flickered!", "green");
                                }
                                else
                                    hub.gameConsoleTransmission.SendToClient("Unlucky", "green");
                                LastSpectatorTrigger = Round.Duration;
                                break;
                            }
                            else if (hub.inventory.CurItem == null)
                                if (Physics.Raycast(hub.PlayerCameraReference.position, hub.PlayerCameraReference.forward, out RaycastHit info, 3.5f))
                                    if (info.collider.TryGetComponent(out HitboxIdentity hbI) && hbI.TargetHub != hub && hbI.TargetHub.roleManager.CurrentRole.RoleTypeId != PlayerRoles.RoleTypeId.Scp173)
                                    {
                                        if (hbI.TargetHub.roleManager.CurrentRole.Team == hub.roleManager.CurrentRole.Team && hub.roleManager.CurrentRole.Team != PlayerRoles.Team.ClassD)
                                            hbI.TargetHub.playerEffectsController.ChangeState<MovementBoost>(5, .1f);
                                        else
                                            hbI.TargetHub.playerEffectsController.ChangeState<Slowness>(5, .1f);
                                    }
                        }
                    }
                    break;


                default:
                    break;
            }
        }
    }
}

