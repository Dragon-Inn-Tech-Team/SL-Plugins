using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;


namespace CustomCommands.Features.Players.Size
{
	public class SizeEvents
	{
		[PluginEvent]
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			ev.Player.ResetSize();
		}

		[PluginEvent]
		public void OnRoundEnd(RoundEndEvent ev)
		{
			foreach (var plr in Server.GetPlayers())
				plr.ResetSize();
		}
	}
}
