﻿using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;
using UnityEngine;

namespace CustomCommands.Commands.Player
{
	[CommandHandler(typeof(PlayerParent))]
	public class Pocket : ICustomCommand
	{
		public string Command => "pocket";

		public string[] Aliases => null;

		public string Description => "Teleports the player into the pocket dimention";

		public string[] Usage { get; } = { "%player%" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.teleporting";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			foreach (var player in players)
				player.Position = Vector3.down * 1998.5f;

			response = $"Teleported {players.Count} {(players.Count == 1 ? "player" : "players")} to the pocket dimension";
			return true;
		}
	}
}
