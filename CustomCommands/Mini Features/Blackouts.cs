using Cassie;
using CustomCommands.Core;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using System.Collections.Generic;
using System.Linq;


namespace CustomCommands.Features.Blackouts
{
	public class Blackouts : CustomFeatureBase
	{
		public Blackouts(bool configSetting) : base(configSetting)
		{
			if (IsEnabled)
			{
				Logger.Info($"Starting blackout manager");

				Timing.CallDelayed(5, () =>
				{
					Timing.RunCoroutine(CheckForLights());
				});
			}
		}


		public static int DelayThisRound;
		public static bool Paused = false, TriggeredThisRound = false;
		public IEnumerator<float> CheckForLights()
		{
			if (!Paused && Round.IsRoundStarted && !Round.IsRoundEnded && Round.Duration.TotalSeconds > DelayThisRound && !TriggeredThisRound)
			{
				Logger.Warn($"Running Blackout");
				TriggeredThisRound = true;
				Timing.RunCoroutine(LightFailure());
			}

			yield return Timing.WaitForSeconds(2);

			Timing.RunCoroutine(CheckForLights());
		}
		public IEnumerator<float> LightFailure()
		{
			if (!Paused)
			{
				TriggeredThisRound = true;

				Announcer.Message(
					new CassieTtsPayload("Attention all personnel . Power malfunction detected . Repair protocol delta 12 activated . Heavy containment zone power termination in 3 . 2 . 1"));

				yield return Timing.WaitForSeconds(18f);

				if (!Round.IsRoundStarted || Round.IsRoundEnded)
					yield break;

				foreach (RoomLightController instance in RoomLightController.Instances)
					if (instance.Room.Zone == MapGeneration.FacilityZone.HeavyContainment)
						instance.ServerFlickerLights(CustomCommandsPlugin.Config.BlackoutDuration);

				foreach (var door in DoorVariant.AllDoors.Where(r => r.IsInZone(MapGeneration.FacilityZone.HeavyContainment)))
					if (door is IDamageableDoor iDD && door.RequiredPermissions.RequiredPermissions == DoorPermissionFlags.None && !door.name.Contains("LCZ"))
						door.NetworkTargetState = true;

				foreach (var tesla in TeslaGate.AllGates)
					tesla.enabled = false;

				yield return Timing.WaitForSeconds(CustomCommandsPlugin.Config.BlackoutDuration);

				if (!Round.IsRoundStarted || Round.IsRoundEnded)
					yield break;

				Announcer.Message(new CassieTtsPayload("Power system repair complete . System back online"));

				foreach (var tesla in TeslaGate.AllGates)
					tesla.enabled = true;
			}

			yield return 0f;
		}

		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev)
		{
			if (RoomLightController.IsInDarkenedRoom(ev.Player.Position) && ev.Door.Permissions == DoorPermissionFlags.None)
				ev.IsAllowed = false;
		}

		public override void OnServerRoundStarted()
		{
			DelayThisRound = UnityEngine.Random.Range(CustomCommandsPlugin.Config.MinBlackoutTime, CustomCommandsPlugin.Config.MaxBlackoutTime);
			TriggeredThisRound = false;
		}
	}
}
