using CommandSystem;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Features.Items.Weapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Grenade : ICustomCommand
	{
		public string Command => "grenadelauncher";

		public string[] Aliases { get; } = { "gl" };

		public string Description => "Launches grenades when you shoot your gun";

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

			if (plr.TemporaryData.Contains("glauncher"))
				plr.TemporaryData.Remove("glauncher");
			else
				plr.TemporaryData.Add("glauncher", string.Empty);

			response = $"Grenade launcher {(plr.TemporaryData.Contains("glauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}
}
