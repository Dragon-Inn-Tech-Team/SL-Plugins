using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Factories;
using PluginAPI.Events;

namespace FriendlyFireDetector
{
    public class Plugin
    {
		[PluginConfig]
		public static Config Config;
		public static bool Paused = false;

		[PluginEntryPoint("Friendly Fire Detector", "1.0.0", "Anti-Friendly Fire system", "ThePheggHerself"), PluginPriority(PluginAPI.Enums.LoadPriority.Lowest)]
		public void OnPluginStart()
		{
			Log.Info($"Plugin is loading...");

			EventManager.RegisterEvents<Handler>(this);
		}
	}
}
