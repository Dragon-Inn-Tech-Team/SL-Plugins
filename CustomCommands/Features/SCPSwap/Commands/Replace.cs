using CommandSystem;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RedRightHand.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPSwap.Commands
{
	[CommandHandler(typeof(SwapParentCommand))]
	public class Replace : ICustomCommand
	{
		public string Command => "replace";

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

				if (SCPSwap.CanHumanSwapToScp(player, out response))
				{
					SCPSwap.QueueSwapHumanToScp(player);

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
