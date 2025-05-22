using CommandSystem;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Features.Items.Weapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Ragdoll : ICustomCommand
	{
		public string Command => "ragdolllauncher";

		public string[] Aliases { get; } = { "rdl" };

		public string Description => "Launches ragdolls when you shoot your gun";

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

			if (plr.TemporaryData.Contains("rdlauncher"))
				plr.TemporaryData.Remove("rdlauncher");
			else
				plr.TemporaryData.Add("rdlauncher", string.Empty);

			response = $"Ragdoll launcher {(plr.TemporaryData.Contains("rdlauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}
}
