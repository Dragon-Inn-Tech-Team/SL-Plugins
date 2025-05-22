using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace TutorialPlus
{
    public class Plugin
    {
        [PluginConfig]
        public static Config Config;

        [PluginEntryPoint("Tutorial Plus", "1.0.4", "Adds config options for tutorial", "PheWitch")]
        public void OnPluginStart()
        {
            EventManager.RegisterEvents<Events>(this);

            Log.Info("Plugin Loaded");
        }
    }
}
