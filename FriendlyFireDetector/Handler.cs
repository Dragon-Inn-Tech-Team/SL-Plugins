using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerRoles;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;
using AdminToys;
using CustomPlayerEffects;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Pickups;
using static RoundSummary;
using Mirror;
using PluginAPI.Events;

namespace FriendlyFireDetector
{
	public class Handler
	{
		public class FFCount : IComparable
		{
			public FFCount(int count)
			{
				Count = count;
			}

			public int Count { get; internal set; }
			public DateTime LastUpdate { get; internal set; }

			public void UpdateCount()
			{
				Count++;
				LastUpdate = DateTime.Now;
			}

			public int CompareTo(object other)
			{
				throw new NotImplementedException();
			}
		}

		public readonly Dictionary<string, List<GrenadeThrowerInfo>> grenadeInfo = new Dictionary<string, List<GrenadeThrowerInfo>>();
		public readonly Dictionary<string, FFInfo> ffInfo = new Dictionary<string, FFInfo>();
		public static bool RoundInProgess = false;

		[PluginEvent(ServerEventType.PlayerSpawn)]
		public void PlayerSpawned(PlayerSpawnEvent args)
		{
			if (args.Player == null || args.Player.IsServer || args.Player.UserId == null || args.Role == RoleTypeId.None)
				return;

			if (args.Player.TemporaryData.Contains("ffdprevrole"))
				args.Player.TemporaryData.Override("ffdprevrole", ((int)args.Player.Role).ToString());
			else
				args.Player.TemporaryData.Add("ffdprevrole", ((int)args.Player.Role).ToString());
		}

		[PluginEvent(ServerEventType.PlayerDamage), PluginPriority(LoadPriority.Highest)]
		public bool PlayerDamageEvent(PlayerDamageEvent args)
		{
			if (Plugin.Paused || !RoundInProgess || args.Target == null || args.Player == null || args.Player.IsServer || args.Player.IsSCP || args.Player.IsTutorial || args.Player.UserId == args.Target.UserId || !(args.DamageHandler is AttackerDamageHandler aDH))
				return true;

			RoleTypeId prevRole = RoleTypeId.None;

			if (args.Player.TemporaryData.TryGet("ffdprevrole", out string roleStr))
				prevRole = (RoleTypeId)int.Parse(roleStr);

			//Checks both the attacker's current role and possible previous roles for if it is considered FF against the target's role
			if (!IsFF(args.Target.Role, args.Player.Role, prevRole))
				return true;

			int friendlies = 0;
			int hostiles = 0;

			foreach (var plr in GetNearbyPlayers(args.Player))
			{
				if (IsFF(plr.Role, args.Player.Role, prevRole))
					friendlies++;
				else
					hostiles++;
			}

			if (hostiles > 0)
			{
				return true;
			}
			else
			{
				args.Player.TemporaryData.Override("ffdstop", $"{aDH.Damage}");

				if (args.Player.TemporaryData.TryGet("ffdcount", out FFCount data))
				{
					data.Count++;
					args.Player.TemporaryData.Override("ffdcount", data);
				}
				else
				{
					args.Player.TemporaryData.Add("ffcount", new FFCount(1));
				}

				return false;
			}
		}

		public bool IsFF(RoleTypeId victim, RoleTypeId attacker, RoleTypeId atkrPrevRole = RoleTypeId.None)
		{
			if ((victim == RoleTypeId.ClassD || IsChaos(victim)) && (attacker == RoleTypeId.ClassD || IsChaos(attacker)))
			{
				if (victim == RoleTypeId.ClassD && attacker == RoleTypeId.ClassD)
					return false;
				return true;
			}
			else if ((victim == RoleTypeId.Scientist || IsMTF(victim)) && (attacker == RoleTypeId.Scientist || IsMTF(attacker)))
				return true;

			if (atkrPrevRole != RoleTypeId.None && atkrPrevRole != RoleTypeId.Spectator)
			{
				if ((victim == RoleTypeId.ClassD || IsChaos(victim)) && (atkrPrevRole == RoleTypeId.ClassD || IsChaos(attacker)))
				{
					if (victim == RoleTypeId.ClassD && atkrPrevRole == RoleTypeId.ClassD)
						return false;
					return true;
				}
				else if ((victim == RoleTypeId.Scientist || IsMTF(victim)) && (attacker == RoleTypeId.Scientist || IsMTF(attacker)))
					return true;
			}

			return false;
		}

		private bool IsChaos(RoleTypeId role)
		{
			switch (role)
			{
				case RoleTypeId.ChaosConscript:
				case RoleTypeId.ChaosRifleman:
				case RoleTypeId.ChaosRepressor:
				case RoleTypeId.ChaosMarauder:
					return true;
				default:
					return false;
			}
		}

		private bool IsMTF(RoleTypeId role)
		{
			switch (role)
			{
				case RoleTypeId.FacilityGuard:
				case RoleTypeId.NtfCaptain:
				case RoleTypeId.NtfSpecialist:
				case RoleTypeId.NtfPrivate:
				case RoleTypeId.NtfSergeant:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Gets a list of all players close to the attacker (100 meters for Surface, 50 for the facility)
		/// </summary>
		/// <returns></returns>
		public List<Player> GetNearbyPlayers(Player atkr, bool rangeOnly = false)
		{
			float distanceCheck = atkr.Position.y > 900 ? 70 : 35;
			List<Player> nearbyPlayers = new List<Player>();

			foreach (var plr in Server.GetPlayers())
			{
				if (plr.IsServer || plr.Role == RoleTypeId.Spectator)
					continue;

				var distance = Vector3.Distance(atkr.Position, plr.Position);

				if (rangeOnly && distance <= distanceCheck)
					nearbyPlayers.Add(plr);
				else
				{
					var angle = Vector3.Angle(atkr.GameObject.transform.forward, atkr.Position - plr.Position);

					if ((distance <= distanceCheck && angle > 130) || distance < 5)
						nearbyPlayers.Add(plr);
				}
			}

			return nearbyPlayers;
		}

		public List<Player> GetNearbyPlayers(Vector3 position)
		{
			float distanceCheck = position.y > 900 ? 70 : 35;
			List<Player> nearbyPlayers = new List<Player>();

			foreach (var plr in Server.GetPlayers())
			{
				if (plr.IsServer || plr.Role == RoleTypeId.Spectator)
					continue;

				var distance = Vector3.Distance(position, plr.Position);

				if (distance <= distanceCheck)
					nearbyPlayers.Add(plr);
			}

			return nearbyPlayers;
		}

		[PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEnd(LeadingTeam team)
		{
			Plugin.Paused = false;
			RoundInProgess = false;
		}

		[PluginEvent(ServerEventType.RoundStart)]
		public void RoundStart()
		{
			RoundInProgess = true;
		}
	}
}
