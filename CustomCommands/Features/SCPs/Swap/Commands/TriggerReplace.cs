using CommandSystem;
using PluginAPI.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Features.SCPs.Swap.Commands
{

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class TriggerReplace : ICustomCommand
	{
		public string Command => "replacescp";

		public string[] Aliases => null;
		public string Description => "Manually triggers the SCP replacement broadcast";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if(Round.Duration > TimeSpan.FromSeconds(SwapManager.SwapToScpSeconds))
			{
				response = $"You can only replace an SCP within the first {SwapManager.SwapToScpSeconds} seconds of the round";
				return false;
			}

			SwapManager.SCPsToReplace++;
			SwapManager.ReplaceBroadcast();
			response = "SCP replace triggered";
			return true;
		}
	}
}
