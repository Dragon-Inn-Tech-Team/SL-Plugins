using CommandSystem;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Features.DoorLocking.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Destroy : ICustomCommand
	{
		public string Command => "playerdestroy";

		public string[] Aliases => null;

		public string Description => "Destroys any door the specified player ineracts with";

		public string[] Usage { get; } = { "%player%" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.playerdoorcontrol";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			foreach (var plr in players)
			{
				if (plr.TemporaryData.Contains("pdest"))
				{
					plr.TemporaryData.Remove("pdest");
				}
				else
					plr.TemporaryData.Add("pdest", string.Empty);
			}

			response = $"Playerdestroy toggled for {players.Count} {(players.Count != 1 ? "players" : "player")}.";
			return true;
		}
	}
}
