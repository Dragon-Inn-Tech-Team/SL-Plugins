using PlayerRoles.RoleAssign;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;


namespace CustomCommands.Features.Humans.LateJoin
{
	public class LateJoinEvents
	{
		[PluginEvent, PluginPriority(LoadPriority.Low)]
		public void PlayerJoin(PlayerJoinedEvent args)
		{
			if (!Plugin.EventInProgress && Round.IsRoundStarted && Round.Duration.TotalSeconds < Plugin.Config.LateJoinTime)
				HumanSpawner.SpawnLate(args.Player.ReferenceHub);
		}
	}
}
