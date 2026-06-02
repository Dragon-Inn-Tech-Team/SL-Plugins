using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.DamageAnnouncements
{
	public class DamageAnnouncements : CustomFeatureBase
	{
		public static Dictionary<string, float> ScpDamage = new Dictionary<string, float>();
		public DamageAnnouncements(bool configSetting) : base(configSetting)
		{
		}

		public override void OnPlayerHurt(PlayerHurtEventArgs ev)
		{
			if (!CustomCommandsPlugin.Config.EnableDamageAnnouncements)
				return;

			var plr = ev.Attacker;
			var trgt = ev.Player;
			if (trgt == null || plr == null || !(Round.IsRoundStarted && ev.DamageHandler is AttackerDamageHandler dmgH && trgt.IsSCP) || dmgH.IsFriendlyFire)
				return;
			if (ScpDamage.ContainsKey(plr.UserId))
				ScpDamage[plr.UserId] += dmgH.Damage;
			else
				ScpDamage.Add(plr.UserId, dmgH.Damage);
		}

		public override void OnServerRoundEnded(RoundEndedEventArgs ev)
		{
			if (!CustomCommandsPlugin.Config.EnableDamageAnnouncements || !ScpDamage.Any())
				return;

			var maxDmg = ScpDamage.Max(x => x.Value);
			var dmg = ScpDamage.OrderByDescending(x => x.Value);
			var str = new List<string>();

			foreach (var kvp in dmg)
			{
				if (str.Count > 2)
					break;

				if (str.Count < 3 && Player.TryGet(kvp.Key, out var plr))
				{
					str.Add($"<size=-14><align=left><pos=-11em>{plr.Nickname}: {kvp.Value}</align></pos></size>");
				}
			}

			if (str.Any())
				Server.SendBroadcast($"<size=-14><align=left><pos=-11em>Most SCP damage this round:</align></pos></size>\n" + string.Join("\n", str), 15);


			ScpDamage.Clear();
		}
	}
}
