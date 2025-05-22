using CommandSystem;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Commands
{
	public class HushEventCommand : ICustomCommand
	{
		public string Command => "hush";

		public string[] Aliases => null;

		public string Description => "Starts the Hush event";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.events";

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var plrs, out var psender))
				return false;

			Plugin.CurrentEvent = EventType.Hush;
			Round.IsLocked = true;

			var room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Hcz049);
			var room049 = room.First();

			room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Hcz939);
			var room939 = room.First();

			foreach (var door in DoorVariant.DoorsByRoom[room049])
			{
				if (door is PryableDoor pryable)
					pryable.ServerChangeLock(DoorLockReason.AdminCommand, true);
			}

			PlayerRoleLoader.TryGetRoleTemplate(RoleTypeId.Scp049, out PlayerRoleBase role);
			(role as IFpcRole).SpawnpointHandler.TryGetSpawnpoint(out Vector3 spawn939, out float rot);

			int scps = Mathf.Clamp((int)Math.Floor((double)(Player.Count / 7)), 1, 4);
			int remainingPlayers = Player.GetPlayers().Count - 1;
			var rand = new System.Random();

			Facility.TurnOffAllLights(10000);

			foreach (Player plr in Player.GetPlayers())
			{
				if (plr.IsServer || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				if ((scps > 0 && rand.Next(0, 2) == 1) || remainingPlayers < scps)
				{
					plr.SetRole(RoleTypeId.Scp939);
					plr.Position = spawn939;

					plr.SendBroadcast("Kill all the humans. The door will open in 10 seconds", 10, shouldClearPrevious: true);
					scps--;
				}
				else
				{
					plr.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Scientist, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
					plr.Position = new Vector3(room939.ApiRoom.Position.x, room939.ApiRoom.Position.y + 1, room939.ApiRoom.Position.z);

					plr.AddItem(ItemType.KeycardMTFCaptain);
					var itemBase = plr.AddItem(ItemType.Flashlight);

					plr.SendBroadcast("Enable all the generators. The SCPs will be released in 10 seconds", 10, shouldClearPrevious: true);

					remainingPlayers--;
				}
			}

			Round.IsLocked = false;

			MEC.Timing.CallDelayed(10, () =>
			{
				foreach (var door in DoorVariant.DoorsByRoom[room049])
				{
					if (door is PryableDoor pryable)
						pryable.NetworkTargetState = true;
				}
			});

			MEC.Timing.CallDelayed(60 * 7, () =>
			{
				if (Plugin.EventInProgress && Plugin.CurrentEvent == EventType.Hush && Round.IsRoundStarted)
					Warhead.Detonate();
			});

			response = $"Hush event has begun";
			return true;
		}
	}
}
