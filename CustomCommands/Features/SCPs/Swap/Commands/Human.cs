using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RedRightHand.Core.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Human : ICustomCommand
	{
		public string Command => "human";

		public string[] Aliases => null;
		public string Description => "Changes you back to a human role";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (SwapManager.CanScpSwapToHuman(player, out response))
				{
					SwapManager.SwapScpToHuman(pSender.ReferenceHub);

					response = "You have now swapped to Human from SCP";
					return true;
				}

				return false;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
