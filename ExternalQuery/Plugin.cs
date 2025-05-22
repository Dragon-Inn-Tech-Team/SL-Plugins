using PluginAPI.Core;
using PluginAPI.Core.Attributes;

namespace ExternalQuery
{
	public class Plugin
	{
		[PluginConfig]
		public static Config Config;
		public static SocketHandler Listener;

		[PluginEntryPoint("External Query", "1.0.0", "Simple re-write of the base QueryProcessor system", "ThePheggHerself")]
		public void OnPluginStart()
		{
			Log.Info($"Plugin is loading...");

			//Registers the events used in the DynamicTags class
			if (!Config.Enabled)
			{
				Log.Info($"Plugin is disabled!");
				return;
			}
			else
			{
				Log.Info($"Plugin is loaded!");
				Listener = new SocketHandler();
			}
		}
	}
}
