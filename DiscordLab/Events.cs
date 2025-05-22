using AdminToys;
using CommandSystem;
using Footprinting;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using LiteNetLib;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin;
using System;
using static RoundSummary;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using Extensions = RedRightHand.Core.Extensions;

namespace DiscordLab
{
	public class Events
	{
		public static DateTime RoundEndTime = new DateTime(), RoundStartTime = new DateTime();
		public static bool RoundInProgress = false;

		#region Warhead

		[PluginEvent(ServerEventType.WarheadDetonation)]
		public void WarheadDetonateEvent() => DiscordLab.Bot.SendMessage(new Msg($"Warhead detonated"));

		[PluginEvent(ServerEventType.WarheadStop)]
		public void WarheadStopEvent(WarheadStopEvent args) => DiscordLab.Bot.SendMessage(new Msg($"Warhead stopped ({args.Player.ToLogString()})"));

		[PluginEvent(ServerEventType.WarheadStart)]
		public void WarheadStartEvent(WarheadStartEvent args)
		{
			if (!args.IsAutomatic)
			{
				if (!args.IsResumed)
					DiscordLab.Bot.SendMessage(new Msg($"Warhead started ({args.Player.ToLogString()})"));
				else
					DiscordLab.Bot.SendMessage(new Msg($"Warhead resumed ({args.Player.ToLogString()}): {Warhead.DetonationTime.ToString("00")}s"));
			}
			else
				DiscordLab.Bot.SendMessage(new Msg($"Warhead started (SERVER)"));
		}

		#endregion


		#region Round

		[PluginEvent(ServerEventType.WaitingForPlayers)]
		public void WaitingForPlayersEvent() => DiscordLab.Bot.SendMessage(new Msg("**Waiting for players...**"));


		[PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEndEvent(RoundEndEvent args)
		{
			RoundInProgress = false;
			RoundEndTime = DateTime.Now;
			DiscordLab.Bot.SendMessage(new Msg($"**Round Ended**" +
				$"\n```Round Time: {new DateTime(TimeSpan.FromSeconds((DateTime.Now - RoundStartTime).TotalSeconds).Ticks):HH:mm:ss}"
				+ $"\nEscaped Class-D: {EscapedClassD}"
				+ $"\nRescued Scientists: {EscapedScientists}"
				+ $"\nSurviving SCPs: {SurvivingSCPs}"
				+ $"\nWarhead Status: {(!Warhead.IsDetonated ? "Not Detonated" : "Detonated")}"
				+ $"\nDeaths: {Kills} ({KilledBySCPs} by SCPs)```"));
		}

		[PluginEvent(ServerEventType.RoundStart)]
		public void RoundStartEvent()
		{
			RoundInProgress = true;
			RoundStartTime = DateTime.Now;
			DiscordLab.Bot.SendMessage(new Msg($"**Round started ({Server.PlayerCount}/{Server.MaxPlayers})**"));
		}

		#endregion


		#region Player

		[PluginEvent(ServerEventType.PlayerHandcuff)]
		public void PlayerDisarmEvent(PlayerHandcuffEvent args) => DiscordLab.Bot.SendMessage(new Msg($"**{args.Player.Nickname} disarmed {args.Target.Nickname}**"));

		[PluginEvent(ServerEventType.PlayerRemoveHandcuffs)]
		public void PlayerUndisarmEvent(PlayerRemoveHandcuffsEvent args) => DiscordLab.Bot.SendMessage(new Msg($"**{args.Player.Nickname} freed {args.Target.Nickname}**"));

		[PluginEvent(ServerEventType.PlayerEscape)]
		public void PlayerEscapeEvent(PlayerEscapeEvent args) => DiscordLab.Bot.SendMessage(new Msg($"{args.Player.Nickname} escaped ({args.Player.Role} -> {args.NewRole})"));

		[PluginEvent(ServerEventType.PlayerThrowProjectile)]
		public void ThrowProjectileEvent(PlayerThrowProjectileEvent args) => DiscordLab.Bot.SendMessage(new Msg($"{args.Thrower.Nickname} threw {args.Item.ItemTypeId}"));

		[PluginEvent(ServerEventType.PlayerJoined)]
		public void PlayerJoinedEvent(PlayerJoinedEvent args) => DiscordLab.Bot.SendMessage(new Msg($"**{args.Player.Nickname} Joined ({args.Player.UserId} \\| ||~~{args.Player.IpAddress}~~|| \\| ID: {args.Player.PlayerId})**"));

		[PluginEvent(ServerEventType.PlayerLeft)]
		public void PlayerLeftEvent(PlayerLeftEvent args)
		{
			if (args.Player.IsServer || !string.IsNullOrEmpty(args.Player.UserId))
				DiscordLab.Bot.SendMessage(new Msg($"{args.Player.Nickname} Left ({args.Player.UserId} \\| ||~~{args.Player.IpAddress}~~|| \\| ID: {args.Player.PlayerId})"));
		}

		[PluginEvent(ServerEventType.PlayerDying)]
		public void PlayerDeathEvent(PlayerDyingEvent args)
		{
			try
			{

				if (args.Player == null || !RoundInProgress)
					return;
				var uDH = args.DamageHandler as UniversalDamageHandler;

				if (args.DamageHandler is AttackerDamageHandler aDH)
				{
					if (aDH.IsSuicide)
						DiscordLab.Bot.SendMessage(new Msg($"{args.Player.Nickname} died to {aDH.GetDamageSource()} (SELF)"));
					else if (aDH.IsFriendlyFire || Extensions.IsFF(args.Player, args.Attacker))
					{
						DiscordLab.Bot.SendMessage(new Msg($"**Teamkill** " +
							$"\n```autohotkey" +
							$"\nPlayer: [{args.Attacker.PlayerId}] {args.Attacker.Role} {args.Attacker.ToLogString()}" +
							$"\nKilled: [{args.Player.PlayerId}] {args.Player.Role} {args.Player.ToLogString()}" +
							$"\nUsing: {aDH.GetDamageSource()}```"));
					}
					else if (args.Player.IsDisarmed && !args.Attacker.ReferenceHub.IsSCP())
					{
						DiscordLab.Bot.SendMessage(new Msg($"__Disarmed Kill__" +
							$"\n```autohotkey" +
							$"\nPlayer: [{args.Attacker.PlayerId}] {args.Attacker.Role} {args.Attacker.ToLogString()}" +
							$"\nKilled: [{args.Player.PlayerId}] {args.Player.Role} {args.Player.ToLogString()}" +
							$"\nUsing: {aDH.GetDamageSource()}```"));
					}
					else
						DiscordLab.Bot.SendMessage(new Msg($"{args.Attacker.Role} {args.Attacker.Nickname} killed {args.Player.Role} {args.Player.Nickname} ({aDH.GetDamageSource()})"));
				}
				else if (args.DamageHandler is WarheadDamageHandler wDH)
					DiscordLab.Bot.SendMessage(new Msg($"Warhead killed {args.Player.ToLogString()}"));
				else
					DiscordLab.Bot.SendMessage(new Msg($"{args.Player.ToLogString()} died ({DeathTranslations.TranslationsById[uDH.TranslationId].LogLabel})"));
			}
			catch (Exception e)
			{
				Log.Info(args.DamageHandler.ServerLogsText);
			}
		}

		[PluginEvent(ServerEventType.PlayerDamage), PluginPriority(LoadPriority.Lowest)]
		public void PlayerDamageEvent(PlayerDamageEvent args)
		{
			if (args.Player == null || args.Target == null || !RoundInProgress)
				return;

			if (args.DamageHandler is AttackerDamageHandler aDH)
			{
				if (aDH.IsSuicide || args.Player.UserId == args.Target.UserId)
				{
					DiscordLab.Bot.SendMessage(new Msg($"{args.Target.Nickname} -> (SELF) {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()})"));
				}
				else if (aDH.IsFriendlyFire || Extensions.IsFF(args.Target, args.Player))
				{
					if (args.Player.TemporaryData.Contains("ffdstop") && args.Player.UserId != args.Target.UserId)
					{
						DiscordLab.Bot.SendMessage(new Msg($"FFD Blocked {args.Player.Nickname} -> {args.Target.Nickname} ({aDH.GetDamageSource()})"));
						args.Player.TemporaryData.Remove("ffdstop");
					}
					else
					{
						DiscordLab.Bot.SendMessage(new Msg($"**{args.Player.Role} {args.Player.ToLogString()} -> {args.Target.Role} {args.Target.ToLogString()} -> {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()})**"));
					}
				}
				else if (args.Target.IsDisarmed && !args.Player.ReferenceHub.IsSCP())
				{
					DiscordLab.Bot.SendMessage(new Msg($"__{args.Player.Role} {args.Player.ToLogString()} -> {args.Target.Role} {args.Target.ToLogString()} -> {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()})__"));
				}
				else
				{
					if (aDH.Damage >= 1)
						DiscordLab.Bot.SendMessage(new Msg($"{args.Player.Nickname} -> {args.Target.Nickname} -> {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()} {aDH.Hitbox})"));
				}
			}
			else if (args.DamageHandler is WarheadDamageHandler wDH)
			{
				DiscordLab.Bot.SendMessage(new Msg($"Warhead damaged {args.Target.ToLogString()} {Math.Round(wDH.Damage, 1)}"));
			}
			else if (args.DamageHandler is UniversalDamageHandler uDH)
			{
				//DiscordLab.Bot.SendMessage(new msgMessage($"{args.Target.ToLogString()} was damaged by {DeathTranslations.TranslationsById[uDH.TranslationId].LogLabel}"));
			}
		}

		[PluginEvent(ServerEventType.PlayerChangeRole)]
		public void RoleChangeEvent(PlayerChangeRoleEvent args)
		{
			if (args.NewRole == RoleTypeId.Spectator || args.NewRole == RoleTypeId.None || args.OldRole.RoleTypeId == RoleTypeId.Spectator || args.OldRole.RoleTypeId == RoleTypeId.None)
				return;

			DiscordLab.Bot.SendMessage(new Msg($"{args.Player.ToLogString()} changed from {args.OldRole.RoleTypeId} to {args.NewRole} ({args.ChangeReason})"));
		}


		[PluginEvent(ServerEventType.PlayerSpawn)]
		public void PlayerSpawnEvent(PlayerSpawnEvent args)
		{
			if (args.Role == RoleTypeId.None || args.Role == RoleTypeId.Spectator || args.Role == RoleTypeId.Overwatch || !RoundInProgress)
				return;
			DiscordLab.Bot.SendMessage(new Msg($"{args.Player.ToLogString()} spawned as {args.Role}"));
		}

		#endregion


		#region Admin

		[PluginEvent(ServerEventType.PlayerMuted)]
		public void PlayerMuteEvent(PlayerMutedEvent args) => DiscordLab.Bot.SendMessage(new Msg($"{args.Issuer.ToLogString()} has {(args.IsIntercom ? "icom-" : "")}muted {args.Player.ToLogString()}"));

		[PluginEvent(ServerEventType.PlayerUnmuted)]
		public void PlayerUnmuteEvent(PlayerUnmutedEvent args) => DiscordLab.Bot.SendMessage(new Msg($"{args.Issuer.ToLogString()} has {(args.IsIntercom ? "icom-" : "")}unmuted {args.Player.ToLogString()}"));

		[PluginEvent(ServerEventType.PlayerBanned)]
		public void PlayerBannedEvent(PlayerBannedEvent args) => DiscordLab.Bot.SendMessage(new Msg($"**New Ban!**```autohotkey" +
			$"\nUser: {args.Player.ToLogString()}" +
			$"\nAdmin: {args.Issuer.ToLogString()}" +
			$"\nDuration: {args.Duration / 60} {(args.Duration / 60 > 1 ? "minutes" : "minute")}" +
			$"\nReason: {(string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason)}```"));

		[PluginEvent(ServerEventType.PlayerKicked)]
		public void PlayerKickedEvent(PlayerKickedEvent args)
		{
			if (!(args.Issuer is PlayerCommandSender pCS))
			{
				DiscordLab.Bot.SendMessage(new Msg($"**Player Kicked!**```autohotkey" +
					$"\nUser: {args.Player.ToLogString()}" +
					$"\nAdmin: SERVER" +
					$"\nReason: {(string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason)}```"));
			}
			else
			{
				var admin = Player.Get(pCS.PlayerId);
				DiscordLab.Bot.SendMessage(new Msg($"**Player Kicked!**```autohotkey" +
					$"\nUser: {args.Player.ToLogString()}" +
					$"\nAdmin: {admin.ToLogString()}" +
					$"\nReason: {(string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason)}```"));
			}
		}

		[PluginEvent(ServerEventType.RemoteAdminCommand)]
		public void RemoteAdminCommandEvent(RemoteAdminCommandEvent args)
		{
			if (args.Sender is ServerConsoleSender SCS)
			{
				DiscordLab.Bot.SendMessage(new Msg($"(CONSOLE) Command run: **{(args.Arguments.Length > 0 ? $"{args.Command} {string.Join(" ", args.Arguments)}" : $"{args.Command}")}**"));
			}

			if (args.Sender is PlayerCommandSender pCS)
			{

				var admin = Player.Get(pCS.PlayerId);

				DiscordLab.Bot.SendMessage(new Msg($"(RA) {admin.ToLogString()} ran: **{(args.Arguments.Length > 0 ? $"{args.Command} {string.Join(" ", args.Arguments)}" : $"{args.Command}")}**"));
			}
		}

		[PluginEvent(ServerEventType.ConsoleCommand)]
		public void ConsoleCommandCommandEvent(ConsoleCommandEvent args)
		{
			DiscordLab.Bot.SendMessage(new Msg($"(CONSOLE) Command run: **{(args.Arguments.Length > 0 ? $"{args.Command} {string.Join(" ", args.Arguments)}" : $"{args.Command}")}**"));
		}

		[PluginEvent(ServerEventType.PlayerGameConsoleCommand)]
		public void PlayerConsoleCommandEvent(PlayerGameConsoleCommandEvent args)
		{
			DiscordLab.Bot.SendMessage(new Msg($"(CLIENT) {args.Player.ToLogString()} ran: **{(args.Arguments.Length > 0 ? $"{args.Command} {string.Join(" ", args.Arguments)}" : $"{args.Command}")}**"));
		}

		#endregion


		#region World

		[PluginEvent(ServerEventType.GrenadeExploded)]
		public void GrenadeExplodeEvent(GrenadeExplodedEvent args)
		{
			if (args.Grenade is ExplosionGrenade expGrenade)
				DiscordLab.Bot.SendMessage(new Msg($"Frag grenade ({(args.Thrower.IsSet ? $"{args.Thrower.Nickname}" : "UNKNOWN")}) exploded"));
			else if (args.Grenade is FlashbangGrenade flshGrenade)
				DiscordLab.Bot.SendMessage(new Msg($"Flashbang ({(args.Thrower.IsSet ? $"{args.Thrower.Nickname}" : "UNKNOWN")}) exploded"));
			else if (args.Grenade is Scp018Projectile scp018)
				DiscordLab.Bot.SendMessage(new Msg($"SCP-018 ({(args.Thrower.IsSet ? $"{args.Thrower.Nickname}" : "UNKNOWN")}) exploded"));
			else
				DiscordLab.Bot.SendMessage(new Msg($"UNKNOWN grenade ({(args.Thrower.IsSet ? $"{args.Thrower.Nickname}" : "UNKNOWN")}) exploded"));
		}

		#endregion

	}
}
