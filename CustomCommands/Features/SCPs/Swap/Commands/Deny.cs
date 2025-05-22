using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RedRightHand.Core.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Deny : ICustomCommand
	{
		public string Command => "scpswapdeny";

		public string[] Aliases { get; } = { "sswapd", "ssd" };
		public string Description => "Denies your pending swap request";

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

				if (!player.TemporaryData.TryGet("swapRequestRecieved", out string UserId))
				{
					response = "You do not have a pending swap request";
					return false;
				}

				if (!Player.TryGet(UserId, out Player swapper))
				{
					response = "Unable to find request sender. Cancelling request";
					player.TemporaryData.Remove("swapRequestRecieved");
					return false;
				}

				swapper.ReceiveHint($"{player.Nickname} denied your swap request", 5);

				player.TemporaryData.Remove("swapRequestRecieved");
				swapper.TemporaryData.Remove("swapRequestSent");

				response = "Request denied";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
