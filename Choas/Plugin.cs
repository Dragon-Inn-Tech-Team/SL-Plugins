using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choas.SSSettings;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using UserSettings.ServerSpecific;

namespace Choas
{
    public class Plugin
    {
        [PluginEntryPoint("Choas", "1.0.0", "brings the choas to SL", "Dragon Inn Tech Team")]
        public void OnPluginStart()
        {
            Log.Info($"Let the choas begin");
            EventManager.RegisterEvents<Events>(this);

            if (ServerSpecificSettingsSync.DefinedSettings == null)
                ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[0];

            ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Concat(CustomSettingsManager.GetAllSettings()).ToArray();
            ServerSpecificSettingsSync.SendToAll();
        }
    }
}
