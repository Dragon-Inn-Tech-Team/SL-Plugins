using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RedRightHand.Core.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Accept : ICustomCommand
	{
		public string Command => "scpswapaccept";

		public string[] Aliases { get; } = { "sswapa", "ssa" };
		public string Description => "Accepts your pending swap request";

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

				if (player.Health != player.MaxHealth)
				{
					response = "You cannot swap as you have taken damage";
					return false;
				}
				else if (Round.Duration > TimeSpan.FromMinutes(1))
				{
					response = "You can only swap your SCP within the first minute of a round";
					return false;
				}

				RoleTypeId playerSCP = player.Role;
				RoleTypeId swapperSCP = swapper.Role;

				swapper.ReceiveHint($"{player.Nickname} accepted your swap request", 5);

				player.SetRole(swapperSCP, RoleChangeReason.LateJoin);
				swapper.SetRole(playerSCP, RoleChangeReason.LateJoin);

				player.TemporaryData.Remove("swapRequestRecieved");
				swapper.TemporaryData.Remove("swapRequestSent");

				response = "Request accepted";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
