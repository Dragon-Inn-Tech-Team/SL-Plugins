﻿using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Commands.Player
{
	public class Trip : ICustomCommand
	{
		public string Command => "ragdoll";

		public string[] Aliases { get; } = { "trip" };

		public string Description => "Ragdolls a specified player";

		public string[] Usage { get; } = { "%player%", "force multiplyer", "ragdoll time (s)" };

		public PlayerPermissions? Permission => PlayerPermissions.Effects;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			float forceMultiplyer = 0;
			float time = 3;

			if (arguments.Count >= 2)
				float.TryParse(arguments.ElementAt(1), out forceMultiplyer);

			if (arguments.Count >= 3)
				float.TryParse(arguments.ElementAt(2), out time);

			foreach (var plr in players)
				plr.RagdollPlayer(time, forceMultiplyer, false);

			response = $"{players.Count} player{(players.Count == 1 ? "s" : "")} ragdolled";

			return true;
		}
	}
}
