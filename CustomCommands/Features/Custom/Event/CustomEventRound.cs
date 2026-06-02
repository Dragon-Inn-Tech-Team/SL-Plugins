using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MapGeneration;
using System;
using System.Linq;

namespace CustomCommands.Features.Custom.Event
{
	public class CustomEventRound : CustomEventsHandler
	{
		RoomName[] AllowedElevators =
		[
			RoomName.HczServers,RoomName.Hcz049, RoomName.HczWarhead
		];

		public TimeSpan RoundTimeLimit = new TimeSpan(0, 10, 00);

		public virtual bool EndEvent(bool force = false)
		{
			if (force || EventEndCondition() || Round.Duration > RoundTimeLimit)
			{
				Logger.Info($"Event round ended");

				return true;
			}

			return false;
		}

		public virtual bool EventEndCondition()
		{
			return true;
		}

		public override void OnServerWaveTeamSelecting(WaveTeamSelectingEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev)
		{
			if (ev.Door.Permissions != Interactables.Interobjects.DoorUtils.DoorPermissionFlags.None)
				ev.IsAllowed = false;
		}

		public override void OnScp914KnobChanging(Scp914KnobChangingEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnScp914Activating(Scp914ActivatingEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev)
		{
			if (!ev.Elevator.Rooms.Any(r => AllowedElevators.Contains(r.Name)))
				ev.IsAllowed = false;
		}

		public override void OnPlayerChangingAttachments(PlayerChangingAttachmentsEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
		{
			if (ev.Pickup.Category == ItemCategory.Firearm || ev.Pickup.Category == ItemCategory.Grenade || ev.Pickup.Category == ItemCategory.SCPItem || ev.Pickup.Category == ItemCategory.SpecialWeapon || ev.Pickup.Category == ItemCategory.Keycard)
			{
				ev.Player.Damage(10, "Cease");
				ev.IsAllowed = false;
			}
		}

		public override void OnServerRoundEndingConditionsCheck(RoundEndingConditionsCheckEventArgs ev)
		{
			ev.CanEnd = EndEvent();
		}
	}
}
