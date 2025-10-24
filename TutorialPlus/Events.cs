using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using LabApi.Events.CustomHandlers;
using LabApi.Events.Arguments.Scp096Events;
using LabApi.Features.Console;
using RedRightHand;
using LabApi.Events.Arguments.Scp173Events;
using LabApi.Events.Arguments.PlayerEvents;
using GameCore;
using LabApi.Features.Permissions;
using PlayerRoles.Spectating;

namespace TutorialPlus
{
	public class Events : CustomEventsHandler
	{
		public override void OnScp096AddingTarget(Scp096AddingTargetEventArgs ev)
		{
			if(!TutorialPlusPlugin.Config.TutorialTrigger096 && ev.Target.Role == RoleTypeId.Tutorial)
			{
				TutorialPlusPlugin.DebugLog($"Plugin blocked player {ev.Target.ToLogString()} from becoming a target of SCP096 {ev.Player.ToLogString()}");
				ev.IsAllowed = false;
			}
		}

		public override void OnScp173AddingObserver(Scp173AddingObserverEventArgs ev)
		{
			if (!TutorialPlusPlugin.Config.TutorialObserve173 && ev.Target.Role == RoleTypeId.Tutorial)
			{
				TutorialPlusPlugin.DebugLog($"Plugin blocked player {ev.Target.ToLogString()} from becoming a target of SCP173 {ev.Player.ToLogString()}");
				ev.IsAllowed = false;
			}
		}

		public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
		{
			if (ev.NewRole.RoleTypeId == RoleTypeId.Tutorial && ev.ChangeReason == RoleChangeReason.RemoteAdmin)
			{
				if (TutorialPlusPlugin.Config.TutorialBypass)
				{
					if (TutorialPlusPlugin.Config.Debug)
						TutorialPlusPlugin.DebugLog($"Plugin enabled bypass mode for {ev.Player.Nickname}");
					ev.Player.ReferenceHub.serverRoles.BypassMode = true;
				}

				if (TutorialPlusPlugin.Config.TutorialGodmode)
				{
					if (TutorialPlusPlugin.Config.Debug)
						TutorialPlusPlugin.DebugLog($"Plugin enabled godmode for {ev.Player.Nickname}");
					ev.Player.ReferenceHub.characterClassManager.GodMode = true;
				}

				if (TutorialPlusPlugin.Config.TutorialNoclip)
				{
					if (TutorialPlusPlugin.Config.Debug)
						TutorialPlusPlugin.DebugLog($"Plugin enabled noclip for {ev.Player.Nickname}");
					FpcNoclip.PermitPlayer(ev.Player.ReferenceHub);
				}

				if (ev.Player.HasPermissions("tutplus.autohide")){
					SpectatableVisibilityManager.SetHidden(ev.Player.ReferenceHub, true);
				}
			}
			else if (ev.OldRole == RoleTypeId.Tutorial)
			{
				if (TutorialPlusPlugin.Config.TutorialBypass)
				{
					if (TutorialPlusPlugin.Config.Debug)
						TutorialPlusPlugin.DebugLog($"Plugin disabled bypass mode for {ev.Player.Nickname}");
					ev.Player.ReferenceHub.serverRoles.BypassMode = false;
				}

				if (TutorialPlusPlugin.Config.TutorialGodmode)
				{
					if (TutorialPlusPlugin.Config.Debug)
						TutorialPlusPlugin.DebugLog($"Plugin disabled godmode for {ev.Player.Nickname}");
					ev.Player.ReferenceHub.characterClassManager.GodMode = false;
				}

				if (TutorialPlusPlugin.Config.TutorialNoclip)
				{
					if (TutorialPlusPlugin.Config.Debug)
						TutorialPlusPlugin.DebugLog($"Plugin disabled noclip for {ev.Player.Nickname}");
					FpcNoclip.UnpermitPlayer(ev.Player.ReferenceHub);
				}

				if (ev.Player.HasPermissions("tutplus.autohide"))
				{
					SpectatableVisibilityManager.SetHidden(ev.Player.ReferenceHub, false);
				}
			}
		}

		public override void OnPlayerCuffing(PlayerCuffingEventArgs ev)
		{
			if (!TutorialPlusPlugin.Config.CuffableTutorial && ev.Target.Role == RoleTypeId.Tutorial)
			{
				TutorialPlusPlugin.DebugLog($"Plugin blocked player {ev.Target.Nickname} from being disarmed by {ev.Player.Nickname}");
				ev.IsAllowed = false;
			}
		}

        public override void OnPlayerIdlingTesla(PlayerIdlingTeslaEventArgs ev)
        {
			if (!TutorialPlusPlugin.Config.TutorialTriggerTesla && ev.Player.Role == RoleTypeId.Tutorial)
			{
				TutorialPlusPlugin.DebugLog($"Plugin blocked player {ev.Player.Nickname} from idling tesla gate in {ev.Tesla.Room} ({ev.Tesla.Position})");
				ev.IsAllowed = false;
			}
		}

        public override void OnPlayerTriggeringTesla(PlayerTriggeringTeslaEventArgs ev)
        {
			if (!TutorialPlusPlugin.Config.TutorialTriggerTesla && ev.Player.Role == RoleTypeId.Tutorial)
			{
				TutorialPlusPlugin.DebugLog($"Plugin blocked player {ev.Player.Nickname} from triggering tesla gate in {ev.Tesla.Room} ({ev.Tesla.Position})");
				ev.IsAllowed = false;
			}
		}
	}
}
