using CommandSystem;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Commands
{
	public class SendHint
	{
		[CommandHandler(typeof(RemoteAdminCommandHandler))]
		public class SendHintCommand : ICustomCommand
		{
			public string Command => "receivehint";

			public string[] Aliases { get; } = { "sendhint", "hint" };

			public string Description => "Sends an administrative hint to specific players.";

			public string[] Usage { get; } = { "%player%", "duration", "message" };

			public PlayerPermissions? Permission => PlayerPermissions.Broadcasting;

			public string PermissionString => string.Empty;

			public bool RequirePlayerSender => false;

			public bool SanitizeResponse => false;

			public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
			{
				if (sender.CanRun(this, arguments, out response, out var plrs, out var _) && float.TryParse(arguments.Array[2], out float duration))
				{
					foreach (var p in plrs)
						p.ReceiveHint(arguments.Array[3], duration);

					response = $"Hint successfully sent to {plrs.Count} player{(plrs.Count == 1 ? "" : "s")}";
					return true;
				}

				return false;
			}
		}
	}
}
