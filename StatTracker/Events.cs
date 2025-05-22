using MEC;
using Newtonsoft.Json;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static StatTracker.Plugin;

namespace StatTracker
{
	public class Events
	{
		public static Dictionary<string, Stats> StatData = new Dictionary<string, Stats>();

		[PluginEvent]
		public void OnPlayerSpawn(PlayerSpawnEvent ev)
		{
			if (ev.Player == null || ev.Player.UserId == null)
				return;

			if(StatData.ContainsKey(ev.Player.UserId) && ev.Player.Role != RoleTypeId.Spectator)
			{
				if (StatData[ev.Player.UserId].Spawns.ContainsKey((int)ev.Player.Role))
					StatData[ev.Player.UserId].Spawns[(int)ev.Player.Role] += 1;
				else
					StatData[ev.Player.UserId].Spawns.Add((int)ev.Player.Role, 1);
			}
		}

		[PluginEvent]
		public void OnPlayerJoin(PlayerJoinedEvent ev)
		{
			if (Round.IsRoundStarted && !Round.IsRoundEnded)
			{
				if (StatData.ContainsKey(ev.Player.UserId))
					StatData[ev.Player.UserId].Jointime = DateTime.UtcNow;
				else
					StatData.Add(ev.Player.UserId, new Plugin.Stats(ev.Player));
			}
		}

		[PluginEvent]
		public void OnRoundStart(RoundStartEvent ev)
		{
			foreach (var plr in Server.GetPlayers())
			{
				if (StatData.ContainsKey(plr.UserId))
					StatData[plr.UserId].Jointime = DateTime.UtcNow;
				else
					StatData.Add(plr.UserId, new Plugin.Stats(plr));
			}
		}

		[PluginEvent]
		public void OnPlayerLeave(PlayerLeftEvent ev)
		{
			try{
				if (Round.IsRoundStarted && !Round.IsRoundEnded)
				{
					if (StatData.ContainsKey(ev.Player.UserId))
					{
						StatData[ev.Player.UserId].SecondsPlayed += (int)(DateTime.UtcNow - StatData[ev.Player.UserId].Jointime).TotalSeconds;
					}
				}
			}
			//To stop it throwing an error if round is null (for like forced round restarts)
			catch(Exception) { }
		}

		[PluginEvent]
		public void OnRoundEnd(RoundEndEvent ev)
		{
			foreach (var plr in Server.GetPlayers())
			{
				if (StatData.ContainsKey(plr.UserId))
				{
					StatData[plr.UserId].SecondsPlayed += (int)(DateTime.UtcNow - StatData[plr.UserId].Jointime).TotalSeconds;

					if (plr.IsAlive)
						StatData[plr.UserId].RoundWon = (plr.Team == LeadingTeamToPlayerTeam(ev.LeadingTeam));
				}
			}

			Timing.RunCoroutine(HandleDataSend(StatData));
		}

		[PluginEvent]
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			StatData.Clear();
		}

		[PluginEvent]
		public void OnPlayerDamage(PlayerDamageEvent ev)
		{
			if (!(ev.DamageHandler is AttackerDamageHandler aDH) || !StatData.ContainsKey(ev.Target.UserId) || !StatData.ContainsKey(aDH.Attacker.Hub.authManager.UserId))
				return;

			var targ = ev.Target;
			var atkr = new Player(aDH.Attacker.Hub);

			if (!aDH.IsFriendlyFire && targ.Role != RoleTypeId.ClassD)
			{
				StatData[targ.UserId].DamageTaken += (int)aDH.Damage;
				StatData[atkr.UserId].DamageDealt += (int)aDH.Damage;
			}
		}

		[PluginEvent]
		public void OnPlayerDeath(PlayerDyingEvent ev)
		{
			if (!(ev.DamageHandler is AttackerDamageHandler aDH) || !StatData.ContainsKey(ev.Player.UserId) || !StatData.ContainsKey(aDH.Attacker.Hub.authManager.UserId))
				return;

			var targ = ev.Player;
			var atkr = new Player(aDH.Attacker.Hub);

			if(StatData[targ.UserId].Deaths.ContainsKey((int)targ.Role))
				StatData[targ.UserId].Deaths[(int)targ.Role] += 1;
			else
				StatData[targ.UserId].Deaths.Add((int)targ.Role, 1);

			if (StatData[atkr.UserId].Killed.ContainsKey((int)targ.Role))
				StatData[atkr.UserId].Killed[(int)targ.Role] += 1;
			else
				StatData[atkr.UserId].Killed.Add((int)targ.Role, 1);
		}

		[PluginEvent]
		public void OnPlayerEscape(PlayerEscapeEvent ev)
		{
			if (StatData.ContainsKey(ev.Player.UserId))
				StatData[ev.Player.UserId].Escaped = true;
		}

		[PluginEvent]
		public void OnPlayerCuffed(PlayerHandcuffEvent ev)
		{
			if (ev.Player == null || ev.Target == null || !StatData.ContainsKey(ev.Player.UserId) || !StatData.ContainsKey(ev.Target.UserId))
				return;

			if (!ev.Target.TemporaryData.StoredData.ContainsKey("st.handcuffed"))
			{
				StatData[ev.Player.UserId].PlayersDisarmed += 1;
				ev.Target.TemporaryData.StoredData.Add("st.handcuffed", null);
			}
		}

		[PluginEvent]
		public void OnPlayerHeal(PlayerUsedItemEvent ev)
		{
			if (ev.Item.Category != ItemCategory.Medical || !StatData.ContainsKey(ev.Player.UserId))
				return;

			StatData[ev.Player.UserId].MedicalItems += 1;
		}

		//Gets the data and sends it off to the API for handling.
		//This is done as a co-routine so that it doesn't interfere with normal server running.

		public IEnumerator<float> HandleDataSend(Dictionary<string, Stats> StatData)
		{
			List<Stats> stats = new List<Stats>();

			foreach (var a in StatData)
			{
				Log.Info($"Adding {a.Key} | {a.Value.UserID} | {a.Value.DNT}");

				if (!a.Value.DNT)
				{
					stats.Add(a.Value);
				}
			}

			Log.Info($"Sending stat data for {stats.Count} players");

			var json = JsonConvert.SerializeObject(stats.ToArray(), Formatting.Indented);
			_ = Post(Plugin.config.ApiEndpoint, new StringContent(json, Encoding.UTF8, "application/json"));

			yield return 0f;
		}

		public async static Task<HttpResponseMessage> Post(string Url, StringContent Content)
		{
			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(Url);

				return await client.PostAsync(client.BaseAddress, Content);
			}
		}

		#region Misc Data Methods

		public Team LeadingTeamToPlayerTeam(RoundSummary.LeadingTeam lTeam)
		{
			switch (lTeam)
			{
				case RoundSummary.LeadingTeam.FacilityForces: return Team.FoundationForces;
				case RoundSummary.LeadingTeam.ChaosInsurgency: return Team.ChaosInsurgency;
				case RoundSummary.LeadingTeam.Anomalies: return Team.ChaosInsurgency;
				default: return Team.Dead;
			}

			#endregion

		}
	}
}
