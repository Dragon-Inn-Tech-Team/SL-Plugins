using CommandSystem;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Features.Items.Weapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Flash : ICustomCommand
	{
		public string Command => "flashlauncher";

		public string[] Aliases { get; } = { "fl" };

		public string Description => "Launches flashbangs when you shoot your gun";

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

			if (plr.TemporaryData.Contains("flauncher"))
				plr.TemporaryData.Remove("flauncher");
			else
				plr.TemporaryData.Add("flauncher", string.Empty);

			response = $"Flashbang launcher {(plr.TemporaryData.Contains("flauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}
}
