using MEC;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace CustomCommands.Features.SCPs.SCP079Removal
{
	public class RemovalEvents
	{
		[PluginEvent]
		public void SpawnEvent(PlayerSpawnEvent args)
		{
			if (args.Role == RoleTypeId.Scp079)
			{
				Timing.CallDelayed(0.15f, () =>
				{
					args.Player.SetRole(RoleTypeId.Scp3114, RoleChangeReason.LateJoin);
				});
			}
		}
	}
}
