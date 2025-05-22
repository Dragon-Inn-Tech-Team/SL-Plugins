using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RedRightHand.Core.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Replace : ICustomCommand
	{
		public string Command => "scp";

		public string[] Aliases => null;
		public string Description => "Replaces you with an SCP who disconnected or swapped to human";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && !pSender.ReferenceHub.IsSCP())
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (SwapManager.CanHumanSwapToScp(player, out response))
				{
					SwapManager.QueueSwapHumanToScp(player);

					response = "You are now in the SCP raffle";
					return true;
				}

				return false;
			}

			response = "You must be a human role or spectator to run this command";
			return false;
		}
	}
}
