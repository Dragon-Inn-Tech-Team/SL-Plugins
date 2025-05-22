using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.SCPs.DamageAnnouncements
{
	public class AnnouncementEvents
	{
		[PluginEvent(ServerEventType.PlayerDamage)]
		public void PlayerDamageEvent(PlayerDamageEvent args)
		{
			if (!Plugin.Config.EnableDamageAnnouncements)
				return;

			var plr = args.Player;
			var trgt = args.Target;
			if (trgt == null || plr == null || Round.IsRoundEnded || !(Round.IsRoundStarted && args.DamageHandler is AttackerDamageHandler dmgH && trgt.IsSCP) || dmgH.IsFriendlyFire)
				return;
			if (AnnouncementManager.ScpDamage.ContainsKey(plr.UserId))
				AnnouncementManager.ScpDamage[plr.UserId] += dmgH.Damage;
			else
				AnnouncementManager.ScpDamage.Add(plr.UserId, dmgH.Damage);
		}

		[PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEndEvent(RoundEndEvent args)
		{
			if (!Plugin.Config.EnableDamageAnnouncements || !AnnouncementManager.ScpDamage.Any())
				return;

			var maxDmg = AnnouncementManager.ScpDamage.Max(x => x.Value);
			var dmg = AnnouncementManager.ScpDamage.OrderByDescending(x => x.Value);
			var str = new List<string>();

			Log.Info($"Damage values: {dmg.Count()}");

			foreach (var kvp in dmg)
			{
				if (str.Count > 2)
					break;

				if (str.Count < 3 && Player.TryGet(kvp.Key, out var plr))
				{
					str.Add($"<size=-14><align=left><pos=-11em>{plr.Nickname}: {kvp.Value}</align></pos></size>");
				}
			}

			Log.Info($"Damage count: {str.Count}");

			if (str.Any())
				Server.SendBroadcast($"<size=-14><align=left><pos=-11em>Most SCP damage this round:</align></pos></size>\n" + string.Join("\n", str), 15);


			AnnouncementManager.ScpDamage.Clear();
		}
	}
}
