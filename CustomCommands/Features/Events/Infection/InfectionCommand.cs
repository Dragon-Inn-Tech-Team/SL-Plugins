using CommandSystem;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Features.Events.Infection
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class InfectionCommand : ICustomCommand
	{
		public string Command => "infection";

		public string[] Aliases => null;

		public string Description => "Enables the infection event round";

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

			int scps = Mathf.Clamp((int)Math.Floor((double)(Player.Count / 5)), 1, 5);
			int remainingPlayers = Player.GetPlayers().Count - 1;
			var rand = new System.Random();


			foreach (Player plr in Player.GetPlayers())
			{
				if (plr.IsServer || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				if ((scps > 0 && rand.Next(0, 2) == 1) || remainingPlayers < scps)
				{
					switch (rand.Next(0, 5))
					{
						default:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp173);
							break;
						case 1:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp049);
							break;
						case 2:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp939);
							break;
						case 4:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp096);
							break;
					}
					plr.Position = new Vector3(room914.ApiRoom.Position.x, room914.ApiRoom.Position.y + 1, room914.ApiRoom.Position.z);
					plr.SendBroadcast("The door will open in 10 seconds. Kill them all", 10, shouldClearPrevious: true);
					scps--;
				}
				else
				{
					plr.SetRole(PlayerRoles.RoleTypeId.ClassD);
				}
				remainingPlayers--;

				MEC.Timing.CallDelayed(10, () =>
				{
					foreach (var door in DoorVariant.DoorsByRoom[room914])
					{
						door.NetworkTargetState = true;
					}
				});
			}

			response = $"Let the infection begin";
			return true;
		}
	}
}
