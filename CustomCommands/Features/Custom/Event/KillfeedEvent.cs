using Achievements.Handlers;
using CustomCommands.Features.EventRounds;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Custom.Event
{
	public abstract class KillfeedEvent : CustomEventRound
	{
		public KillfeedEntry[] KillfeedEntries = new KillfeedEntry[3];

		public virtual string GetMurderType(DamageHandlerBase dhb)
		{
			if (dhb is FirearmDamageHandler)
				return "shot";
			else if (dhb is ExplosionDamageHandler)
				return "exploded";
			else if (dhb is MicroHidDamageHandler)
				return "shocked";
			else if (dhb is RecontainmentDamageHandler)
				return "recontained";
			else if (dhb is Scp018DamageHandler)
				return "balled";
			else if (dhb is Scp049DamageHandler)
				return "cured";
			else if (dhb is Scp096DamageHandler)
				return "eviscerated";
			else if (dhb is Scp939DamageHandler)
				return "shredded";
			else if (dhb is Scp3114DamageHandler)
				return "fooled";
			else if (dhb is DisruptorDamageHandler)
				return "distrupted";
			else if (dhb is JailbirdDamageHandler)
				return "bonked";
			else return "killed";
		}

		public virtual string TeamToColour(Team team)
		{
			switch (team)
			{
				default: return "white";
				case Team.FoundationForces: return "blue";
				case Team.Scientists: return "yellow";
				case Team.Flamingos: return "pink";
				case Team.SCPs: return "red";
				case Team.ChaosInsurgency: return "green";
				case Team.ClassD: return "orange";
			}
		}
		
		public virtual KillfeedEntry CreateEntry(PlayerDyingEventArgs ev)
		{
			return new KillfeedEntry($"<color={TeamToColour(ev.Attacker.RoleBase.Team)}>{ev.Attacker.DisplayName}</color> {GetMurderType(ev.DamageHandler)} <color={TeamToColour(ev.Player.RoleBase.Team)}>{ev.Player.DisplayName}</color>", Round.Duration);
		}

		public virtual void AddKillfeedEntry(KillfeedEntry entry)
		{
			KillfeedEntries[2] = KillfeedEntries[1];
			KillfeedEntries[1] = KillfeedEntries[0];
			KillfeedEntries[0] = entry;

			UpdateKillfeed();
		}

		public virtual void UpdateKillfeed()
		{
			string killfeedString = "";
			foreach(KillfeedEntry entry in KillfeedEntries)
				killfeedString += $"<size=-14><align=left><pos=-8em>{entry.EntryMessage}</align></pos></size>\n";

			Server.ClearBroadcasts();
			Server.SendBroadcast(killfeedString, 5);
		}

		public struct KillfeedEntry
		{
			public string EntryMessage { get; set; }
			public TimeSpan EntryTime { get; set; }
			public KillfeedEntry(string entryMessage, TimeSpan entryTime)
			{
				EntryMessage = entryMessage;
				EntryTime = entryTime;
			}
		}
	}
}
