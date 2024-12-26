﻿using CustomCommands.Events;
using CustomCommands.Features.Map;
using CustomCommands.Features.Map.RollingBlackouts;
using CustomCommands.ServerSettings;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using UserSettings.ServerSpecific;

namespace CustomCommands
{
	public enum EventType
	{
		NONE = 0,

		Infection = 1,
		Battle = 2,
		Hush = 3,
		SnowballFight = 4 // This event is christmas-exclusive.
	}
	public enum VoteType
	{
		NONE = 0,
		AdminVote = 1,
		AutoEventVote = 2,
	}


	public class Plugin
	{
		[PluginConfig]
		public static Config Config;

		public static bool EventInProgress => CurrentEvent != EventType.NONE;
		public static EventType CurrentEvent = EventType.NONE;

		[PluginEntryPoint("Custom Commands", "1.0.0", "Simple plugin for custom commands", "ThePheggHerself")]
		public void OnPluginStart()
		{
			Harmony harmony = new Harmony("CC-Patching-Phegg");
			harmony.PatchAll();

			Log.Info($"Plugin is loading...");

            EventManager.RegisterEvents<Carpincho>(this);

            if (Config.EnableDoorLocking)
			{
				EventManager.RegisterEvents<Features.DoorLocking.LockingEvents>(this);
			}

			if (Config.EnableEvents)
			{
				EventManager.RegisterEvents<Features.Events.GlobalEvents>(this);
				EventManager.RegisterEvents<Features.Events.Infection.InfectionEvents>(this);
			}

			if (Config.EnableBetterDisarming)
			{
				EventManager.RegisterEvents<Features.Humans.Disarming.DisarmingEvents>(this);
			}

			if (Config.EnableLateJoin)
			{
				EventManager.RegisterEvents<Features.Humans.LateJoin.LateJoinEvents>(this);
			}

			if (Config.EnableLateSpawn)
			{
				EventManager.RegisterEvents<Features.Humans.LateSpawn.LateSpawnEvents>(this);
			}

			if (Config.EnableTutorialFixes)
			{
				EventManager.RegisterEvents<Features.Humans.TutorialFix.TutorialEvents>(this);
			}

			if (Config.EnableSpecialWeapons)
			{
				EventManager.RegisterEvents<Features.Items.Weapons.WeaponEvents>(this);
			}

			if (Config.EnableAdditionalSurfaceLighting)
			{
				EventManager.RegisterEvents<Features.Map.SurfaceLightFix.LightFixEvents>(this);
			}

			if (Config.EnableDamageAnnouncements)
			{
				EventManager.RegisterEvents<Features.SCPs.DamageAnnouncements.AnnouncementEvents>(this);
			}

			if (Config.EnableScp079Removal)
			{
				EventManager.RegisterEvents<Features.SCPs.SCP079Removal.RemovalEvents>(this);
				EventManager.RegisterEvents<Features.SCPs.SCP3114.SCP3114Overhaul>(this);
			}

			if (Config.EnableScpSwap)
			{
				EventManager.RegisterEvents<Features.SCPs.Swap.SwapEvents>(this);
			}

			if (Config.EnableDebugTests)
			{
				EventManager.RegisterEvents<Features.Testing.Navigation.NavigationEvents>(this);
				EventManager.RegisterEvents<Features.Testing.TestingDummies>(this);
			}

			if (Config.EnablePlayerVoting)
			{
				EventManager.RegisterEvents<Features.Voting.VotingEvents>(this);
			}

			if (Config.EnableWeeklyEvents)
			{
				EventManager.RegisterEvents<Features.Events.WeeklyEvents.EventManager>(this);
				EventManager.RegisterEvents<Features.Events.WeeklyEvents.Events>(this);
			}

			if(Config.EnableBlackout)
			{
				EventManager.RegisterEvents<BlackoutEvents>(this);
				new BlackoutManager();
			}

			ServerSpecificSettingsSync.DefinedSettings = CustomSettingsManager.GetAllSettings();
			ServerSpecificSettingsSync.SendToAll();

			RagdollManager.OnRagdollSpawned += Features.Ragdoll.PocketRagdollHandler.RagdollManager_OnRagdollSpawned;

			EventManager.RegisterEvents<Features.Players.Size.SizeEvents>(this);
		}
	}
}
