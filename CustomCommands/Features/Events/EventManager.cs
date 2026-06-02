using CustomCommands.Core;
using CustomCommands.Features.Custom.Event;
using CustomCommands.Features.EventRounds.Events;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MapGeneration;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.EventRounds
{
	public class EventManager : CustomFeatureBase
	{
		private static Dictionary<string, CustomEventRound> EventHandlers = new Dictionary<string, CustomEventRound>()
		{
			{ "hideandseek", new HeavyHideAndSeek() },
			{ "infection", new SCPInfection() },
			{ "tdm", new TeamDeathMatch() }
		};

		private static KeyValuePair<string, CustomEventRound>? QueuedEvent = null;
		public static bool EventInProgress => CurrentEvent == null && (Round.IsRoundStarted && !Round.IsRoundEnded);
		private static CustomEventRound CurrentEvent;

		public EventManager(bool configSetting) : base(configSetting)
		{
		}

		public static bool QueueEvent(string type)
		{
			if (EventHandlers.ContainsKey(type))
			{
				QueuedEvent = new KeyValuePair<string, CustomEventRound>(type, EventHandlers[type]);

				if (!Round.IsRoundStarted)
					StartEvent();

				return true;
			}
			return false;
		}

		public static void StartEvent()
		{
			if (QueuedEvent.HasValue)
			{
				Server.SendBroadcast($"The {QueuedEvent.Value.Key} event will run this round", 5);

				//Round.IsLobbyLocked = true;
				CurrentEvent = QueuedEvent.Value.Value;

				//MEC.Timing.CallDelayed(15f, () =>
				//{
				//	Round.IsLobbyLocked = false;
				//});

				QueuedEvent = null;
			}
			else
				CurrentEvent = null;
		}

		public override void OnServerWaitingForPlayers() => StartEvent();

		public static Room GetRandomRoom(params RoomName[] rooms)
		{
			var roomIndex = 0;
			if (rooms.Length > 1)
				roomIndex = new System.Random().Next(rooms.Length);

			return Room.Get(rooms[roomIndex]).First();
		}

		public override void OnServerRoundStarted() => CurrentEvent?.OnServerRoundStarted();
		public override void OnServerRoundEndingConditionsCheck(RoundEndingConditionsCheckEventArgs ev) => CurrentEvent?.OnServerRoundEndingConditionsCheck(ev);
		public override void OnServerWaveTeamSelecting(WaveTeamSelectingEventArgs ev) => CurrentEvent?.OnServerWaveTeamSelecting(ev);
		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev) => CurrentEvent?.OnPlayerInteractingDoor(ev);
		public override void OnScp914KnobChanging(Scp914KnobChangingEventArgs ev) => CurrentEvent?.OnScp914KnobChanging(ev);
		public override void OnScp914Activating(Scp914ActivatingEventArgs ev) => CurrentEvent?.OnScp914Activating(ev);
		public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev) => CurrentEvent?.OnPlayerInteractingElevator(ev);
		public override void OnPlayerDying(PlayerDyingEventArgs ev) => CurrentEvent?.OnPlayerDying(ev);
		public override void OnPlayerDeath(PlayerDeathEventArgs ev) => CurrentEvent?.OnPlayerDeath(ev);
		public override void OnPlayerChangingAttachments(PlayerChangingAttachmentsEventArgs ev) => CurrentEvent?.OnPlayerChangingAttachments(ev);
	}
}
