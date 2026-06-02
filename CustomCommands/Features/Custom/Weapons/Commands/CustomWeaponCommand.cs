using CommandSystem;
using CustomCommands.Core;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;
using System.Linq;

namespace CustomCommands.Features.Custom.Weapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CustomWeaponCommand : ICustomCommand
	{
		public string PermissionString => string.Empty;
		public PlayerPermissions? Permission => null;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public string Command => "customweapon";

		public string[] Aliases => ["cw"];

		public string Description => "Give yourself a custom weapon";

		public string[] Usage => ["weapon"];

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out var pSender) || !arguments.TryGetCommandArgument(0, string.Empty, out var weaponType, out response))
				return false;

			if (CustomWeaponsManager.AvailableWeapons.TryGetValue(weaponType.ToLowerInvariant(), out CustomWeaponBase wepBase))
			{
				wepBase.GiveWeapon(Player.Get(pSender.ReferenceHub));

				response = $"You have been given a {wepBase.Name}";
				return true;
			}
			else
			{
				response = $"Unable to find role {arguments.ElementAt(1)}. It may not exist, or hasn't been enabled";
				return false;
			}
		}
	}
}
