using CommandSystem;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Features.Events.TDMInfection
{
	public class TDMInfectionCommand : ICustomCommand
	{
		public string Command => "tdm";

		public string[] Aliases { get; } = { "deathmatch", "teamdeathmatch" };

		public string Description => "Starts an infection TDM event";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.events";

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var plrs, out var psender))
				return false;

			Plugin.CurrentEvent = EventType.Infection;
			Round.IsLocked = true;

			var room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Lcz914);
			var room914 = room.First();

			foreach (var door in DoorVariant.DoorsByRoom[room914])
			{
				door.ServerChangeLock(DoorLockReason.AdminCommand, true);
			}

			room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Lcz330);
			var room330 = room.First();

			foreach (var door in DoorVariant.DoorsByRoom[room330])
			{
				door.ServerChangeLock(DoorLockReason.AdminCommand, true);
			}

			int index = 0;
			foreach (Player plr in Player.GetPlayers())
			{
				if (plr.IsServer || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;
				index++;
				if (index % 2 == 0)
				{
					plr.SetRole(PlayerRoles.RoleTypeId.NtfCaptain);
					plr.Position = new Vector3(room914.ApiRoom.Position.x, room914.ApiRoom.Position.y + 1, room914.ApiRoom.Position.z);
					plr.SendBroadcast("The door will open in 10 seconds. Kill all chaos", 10, shouldClearPrevious: true);
				}
				else
				{
					plr.SetRole(PlayerRoles.RoleTypeId.ChaosMarauder);
					plr.Position = new Vector3(room330.ApiRoom.Position.x, room330.ApiRoom.Position.y + 1, room330.ApiRoom.Position.z);
					plr.SendBroadcast("The door will open in 10 seconds. Kill all MTF", 10, shouldClearPrevious: true);
				}

				MEC.Timing.CallDelayed(10, () =>
				{
					foreach (var door in DoorVariant.DoorsByRoom[room914])
					{
						door.NetworkTargetState = true;
					}

					foreach (var door in DoorVariant.DoorsByRoom[room330])
					{
						door.NetworkTargetState = true;
					}
				});
			}

			Round.IsLocked = false;

			response = $"Let the fight commence";
			return true;
		}
	}
}
