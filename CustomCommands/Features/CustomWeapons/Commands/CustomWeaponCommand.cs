using CommandSystem;
using PlayerRoles.FirstPersonControl;
using RedRightHand;
using RedRightHand.Commands;
using RedRightHand.CustomRoles;
using RedRightHand.CustomWeapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Features.CustomWeapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CustomWeaponCommand : ICustomCommand
	{
		public PlayerPermissions? Permission => PlayerPermissions.ForceclassWithoutRestrictions;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public string Command => "customweapon";

		public string[] Aliases => ["cw"];

		public string Description => "Spawns a custom weapon";

		public string[] Usage => ["weapon"];

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out var pSender))
				return false;

			var weaponType = arguments.ElementAt(0);
			var spawnPos = new Vector3(pSender.ReferenceHub.GetPosition().x, pSender.ReferenceHub.GetPosition().y + 1, pSender.ReferenceHub.GetPosition().z);

			if (CustomWeaponManager.SpawnCustomWeapon(weaponType, spawnPos, out var customWeapon))
			{
				response = $"Custom weapon {weaponType} spawned at {spawnPos}";
				return true;
			}
			else
			{
				response = $"Unable to find weapon {weaponType}. It may not exist, or hasn't been enabled";
				return false;
			}
		}
	}
}
