using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;
using System.Linq;
using Utils;

namespace CustomCommands.Commands.Player
{
	[CommandHandler(typeof(PlayerParent))]
	public class SendTo : ICustomCommand
	{
		public string Command => "sendtoplayer";

		public string[] Aliases { get; } = { "send" };

		public string Description => "Teleports a player to another";

		public string[] Usage { get; } = { "%player%", "%target%" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.teleporting";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			var index = Usage.IndexOf("%target%");
			var hubs = RAUtils.ProcessPlayerIdOrNamesList(arguments, index, out _, false);
			LabApi.Features.Wrappers.Player target;

			if (hubs.Count < 1)
			{
				response = $"No player(s) found for: {arguments.ElementAt(index)}";
				return false;
			}
			else
			{
				target = LabApi.Features.Wrappers.Player.Get(hubs.First());
			}

			foreach (var plr in players)
			{
				plr.Position = target.Position;
			}

			response = $"Teleported {players.Count} {(players.Count == 1 ? "player" : "players")} to position {target.Nickname} ({target.PlayerId})";
			return true;
		}
	}
}
