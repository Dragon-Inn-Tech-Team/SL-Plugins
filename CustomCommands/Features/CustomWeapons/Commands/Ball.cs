﻿using CommandSystem;
using CustomCommands.Features.CustomWeapons.Weapons;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.CustomWeapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Ball : ICustomCommand
	{
		public string Command => "balllauncher";

		public string[] Aliases { get; } = { "bl" };

		public string Description => "Launches balls when you shoot your gun";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
				return false;

			var plr = Player.Get(pSender.ReferenceHub);
		
			if(!CustomWeapons.AvailableWeapons.TryGetValue(CustomWeapons.WeaponType.Ball, out var customWeaponBase))
			{
				response = $"Unable to find weapon with type {CustomWeapons.WeaponType.Ball}";
				return false;
			}

			bool enabled = customWeaponBase.ToggleWeapon(plr);


			response = $"Ball launcher {(enabled ? "enabled" : "disabled")}.";
			return true;
		}
	}
}
