using CommandSystem;
using InventorySystem.Disarming;
using PlayerStatsSystem;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using System.Linq;
using Utils;

namespace CustomCommands.Commands.Misc
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Mock : ICustomCommand
	{
		public string Command => "mock";

		public string[] Aliases => null;

		public string Description => "Mocks an event triggered by a specific player";

		public string[] Usage { get; } = { "type", "player", "player2" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.mock";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			var attacker = RAUtils.ProcessPlayerIdOrNamesList(arguments, 1, out _, false);
			var target = RAUtils.ProcessPlayerIdOrNamesList(arguments, 2, out _, false);

			if (attacker.Count > 0 && target.Count > 0)
			{
				switch (arguments.ElementAt(0).ToLower())
				{
					default:
						{
							target[0].playerStats.DealDamage(new ExplosionDamageHandler(new Footprinting.Footprint(attacker[0]), new UnityEngine.Vector3(1, 1, 1), 1000, 1000, ExplosionType.PinkCandy));

							break;
						}
					case "disarm":
						{
							DisarmingHandlers.InvokeOnPlayerDisarmed(attacker[0], target[0]);
							target[0].inventory.SetDisarmedStatus(attacker[0].inventory);

							break;
						}
				}

				response = $"Mocked event {arguments.ElementAt(0)} as {arguments.ElementAt(1)} against {arguments.ElementAt(2)}";

				return true;
			}

			response = $"Unable to mock {arguments.ElementAt(0)} as player(s) could not be found";

			return true;
		}
	}
}
