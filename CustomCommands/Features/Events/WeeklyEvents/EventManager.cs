using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;

namespace CustomCommands.Features.Events.WeeklyEvents
{
	public enum EventType
	{
		/// <summary>
		/// No active event.
		/// </summary>
		NONE = 0,

		/// <summary>
		/// Flipping coins will cause a fake explosion and kill the flipper if it lands on tails.
		/// </summary>
		CoinFlipDeath,

		/// <summary>
		/// Players who get disarmed gain 3 seconds of super speed.
		/// </summary>
		SpeedyDisarm = 2,

		/// <summary>
		/// Causes the current room to change colour when a flashlight is turned on.
		/// </summary>
		FlashlightDisco,

		/// <summary>
		/// Randomizes the effect of candy.
		/// </summary>
		//CandyRandomizer,

		/// <summary>
		/// Steals the HP of the human target shot with this gun.
		/// </summary>
		HealthStealer15,

		/// <summary>
		/// Teleports the player (must be human) to a different room when hit with a flashbang.
		/// </summary>
		//FlashbangTeleport

		/// <summary>
		/// Upgrades all cards if landing on heads, downgrades if landing on tails.
		/// </summary>
		CoinCardUpgrade,
	}
	public class EventManager
	{
		public static EventType CurrentEvent;

		[PluginEvent]
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			var e = IsWeekend();

			if (IsWeekend() && CurrentEvent == EventType.NONE)
			{
				CurrentEvent = EventType.CoinFlipDeath;
				//CurrentEvent = (EventType)UnityEngine.Random.Range(0, 6);

				Log.Info(CurrentEvent.ToString());
			}
		}

		public bool IsWeekend()
		{
			return DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday;
		}
	}
}
