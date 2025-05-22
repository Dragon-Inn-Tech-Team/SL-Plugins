using Interactables.Interobjects.DoorUtils;
using MEC;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.Map.RollingBlackouts
{
	public class BlackoutManager
	{
		BlackoutManager Instance;

		public BlackoutManager()
		{
			Instance = this;

			Log.Info($"Starting blackout manager");

			Timing.CallDelayed(5, () =>
			{
				Timing.RunCoroutine(CheckForLights());
			});

		}


		public static int DelayThisRound;
		public static bool Pause, TriggeredThisRound;
		public IEnumerator<float> CheckForLights()
		{
			if (!Pause && Round.IsRoundStarted && !Round.IsRoundEnded && Round.Duration.TotalSeconds > DelayThisRound && !TriggeredThisRound)
			{
				Log.Warning($"Running Blackout");
				TriggeredThisRound = true;
				Timing.RunCoroutine(LightFailure());
			}

			yield return Timing.WaitForSeconds(2);

			Timing.RunCoroutine(CheckForLights());
		}

		public IEnumerator<float> LightFailure()
		{
			TriggeredThisRound = true;
			Cassie.Message("Attention all personnel . Power malfunction detected . Repair protocol delta 12 activated . Heavy containment zone power termination in 3 . 2 . 1", false, true, true);
			yield return Timing.WaitForSeconds(18f);

			if (!Round.IsRoundStarted || Round.IsRoundEnded)
				yield break;

			foreach (RoomLightController instance in RoomLightController.Instances)
				if (instance.Room.Zone == MapGeneration.FacilityZone.HeavyContainment)
					instance.ServerFlickerLights(Plugin.Config.BlackoutDuration);

			foreach (var door in DoorVariant.AllDoors.Where(r => r.IsInZone(MapGeneration.FacilityZone.HeavyContainment)))
				if (door is IDamageableDoor iDD && door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None && !door.name.Contains("LCZ"))
					door.NetworkTargetState = true;

			foreach (var tesla in TeslaGate.AllGates)
				tesla.enabled = false;

			yield return Timing.WaitForSeconds(Plugin.Config.BlackoutDuration);

			if (!Round.IsRoundStarted || Round.IsRoundEnded)
				yield break;

			Cassie.Message("Power system repair complete . System back online", false, true, true);

			foreach (var tesla in TeslaGate.AllGates)
				tesla.enabled = true;

			yield return 0f;
		}
	}
}
