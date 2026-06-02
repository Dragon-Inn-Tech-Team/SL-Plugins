using CommandSystem;
using CustomCommands.Commands.Player;
using LabApi.Features.Console;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.DoorLocking.Commands
{
	[CommandHandler(typeof(PlayerParent))]
	public class Doors : ICustomCommand
	{
		public string Command => "doors";

		public string[] Aliases => null;

		public string Description => "Toggle door controls for player(s)";

		public string[] Usage { get; } = { "%player%", "type" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.playerdoorcontrol";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			Logger.Info($"1111 {DoorLocking.Instance.IsEnabled}");

			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			Logger.Info("2222");

			if (!arguments.TryGetCommandArgument(1, string.Empty, out var lockTypeString, out response))
				return false;

			Logger.Info("3333");

			DoorLocking.LockType type;

			Logger.Info("4444");

			switch (lockTypeString)
			{
				default:
					type = DoorLocking.LockType.NONE;
					break;
				case "d":
				case "destroy":
				case "break":
				case "explode":
					type = DoorLocking.LockType.Destroy;
					break;
				case "lock":
				case "l":
				case "deny":
					type = DoorLocking.LockType.Lock;
					break;
			}

			Logger.Info("AAA");


			foreach (var plr in players)
			{
				Logger.Info("BBB");

				if (type == DoorLocking.LockType.NONE)
				{
					Logger.Info("CCC");

					if (DoorLocking.LockingDict.ContainsKey(plr.UserId))
						DoorLocking.LockingDict.Remove(plr.UserId);
				}
				else
					DoorLocking.LockingDict.AddToOrReplaceValue(plr.UserId, type);
			}

			response = $"Doorlocking set to {type} for {players.Count} {(players.Count != 1 ? "players" : "player")}.";
			return true;
		}
	}
}
