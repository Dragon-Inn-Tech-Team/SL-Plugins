using CommandSystem;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Features.Items.Weapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Ball : ICustomCommand
	{
		public string Command => "balllauncher";

		public string[] Aliases { get; } = { "bl" };

		public string Description => "Launches balls when you shoot your gun";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
				return false;

			var plr = Player.Get(pSender.ReferenceHub);

			if (plr.TemporaryData.Contains("blauncher"))
				plr.TemporaryData.Remove("blauncher");
			else
				plr.TemporaryData.Add("blauncher", string.Empty);

			response = $"Ball launcher {(plr.TemporaryData.Contains("blauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}
}
