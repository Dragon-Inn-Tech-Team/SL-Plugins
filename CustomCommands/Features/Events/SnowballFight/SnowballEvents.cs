using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace CustomCommands.Features.Events.SnowballFight
{
	public class SnowballEvents
	{
		[PluginEvent]
		public void PlayerDeath(PlayerDeathEvent args)
		{
			if (Plugin.CurrentEvent == EventType.SnowballFight)
			{
				args.Attacker.Heal(15);
			}
		}
	}
}
