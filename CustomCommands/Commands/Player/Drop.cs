using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;


namespace CustomCommands.Commands.Player
{
	[CommandHandler(typeof(PlayerParent))]
	public class Drop : ICustomCommand
	{
		public string Command => "drop";

		public string[] Aliases { get; } = ["dropall", "dropinv", "strip"];

		public string Description => "Drops all items and ammo from the specified player(s)";

		public string[] Usage { get; } = ["%player%"];

		public PlayerPermissions? Permission => PlayerPermissions.PlayersManagement;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			foreach (var plr in players)
				plr.DropEverything();

			response = $"Player {(players.Count > 1 ? "inventories" : "inventory")} dropped";

			return true;
		}
	}
}
