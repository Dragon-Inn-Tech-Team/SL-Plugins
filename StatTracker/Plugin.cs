using System;
using System.Collections.Generic;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace StatTracker
{
    public class Plugin
    {
		[PluginConfig]
		public static Config config;

		[PluginEntryPoint("Stat Tracker", "1.0.0", "Tracks player stats", "PheWitch")]
		public void OnPluginStart()
		{
			EventManager.RegisterEvents<Events>(this);

			Log.Info($"Plugin Loaded! Endpoint: {config.ApiEndpoint}");
		}


		public class Stats
		{
			public Stats(Player plr)
			{
				UserID = plr.UserId;
				DNT = plr.DoNotTrack;
				Jointime = DateTime.UtcNow;
			}

			public string UserID; 
			public bool DNT = true;
			public int MedicalItems = 0; 
			public bool Escaped = false; //Has the player escaped
			public bool RoundWon = false; //Is the player alive as the winning team when the round ends
			public int SecondsPlayed = 0; //How many seconds the player has played for
			public int PlayersDisarmed = 0; //How many people has the player disarmed
			public int DamageDealt = 0; //How much damage the player has dealt
			public int DamageTaken = 0; //How much damage the player has taken
			public Dictionary<int, int> Spawns = new Dictionary<int, int>(); //How many times this player has spawned as a role
			public Dictionary<int, int> Kills = new Dictionary<int, int>(); //How many kills this player has as this role
			public Dictionary<int, int> Killed = new Dictionary<int, int>(); //How many times this player has killed this role
			public Dictionary<int, int> Deaths = new Dictionary<int, int>(); //How many times this player has died as this role
			public DateTime Jointime;
		}
	}
}
