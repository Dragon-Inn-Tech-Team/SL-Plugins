using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles.RoleAssign;
using RedRightHand;

namespace CustomCommands.Features.LateJoin
{
	public class LateJoin : CustomFeatureBase
	{
		public static bool LateJoinPaused = false;
		public LateJoin(bool configSetting) : base(configSetting)
		{
		}

		public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
		{
			if (LateJoinPaused)
				return;

			if (Extensions.RoundInProgress() && Round.Duration.TotalSeconds < CustomCommandsPlugin.Config.LateJoinTime)
				HumanSpawner.SpawnLate(ev.Player.ReferenceHub);
		}
	}
}
