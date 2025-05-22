using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using RemoteAdmin;
using System;
using System.Linq;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Swap : ICustomCommand
	{
		public string Command => "scpswap";

		public string[] Aliases { get; } = { "sswap" };
		public string Description => "Changes your current SCP";

		public string[] Usage { get; } = { "scp" };

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

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

				var role = Extensions.GetRoleFromString($"SCP" + arguments.Array[1]);
				if (SwapManager.AvailableSCPs.Contains(role))
				{
					response = "You cannot swap to that SCP";
					return false;
				}

				var scpNum = player.Role.SCPNumbersFromRole();
				var target = Player.GetPlayers().Where(r => r.Role == role).First();

				if (player.TemporaryData.Contains("swapRequestSent"))
				{
					response = "You already have another pending swap request";
					return false;
				}
				else if (player.TemporaryData.Contains("swapRequestRecieved"))
				{
					response = "You must reject your pending request before trying to swap with another SCP";
					return false;
				}
				else if (target.TemporaryData.Contains("swapRequestSent"))
				{
					response = $"{target} is trying to swap with another player";
					return false;
				}
				else if (target.TemporaryData.Contains("swapRequestRecieved"))
				{
					response = $"{target} already has a pending swap request";
					return false;
				}

				target.ReceiveHint($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to SCP-{scpNum}, or type `.sswapd` to reject the request", 8);
				target.SendConsoleMessage($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to SCP-{scpNum}, or type `.sswapd` to reject the request");
				target.TemporaryData.Add("swapRequestRecieved", player.UserId);
				player.TemporaryData.Add("swapRequestSent", target.UserId);

				response = "Swap Request Sent";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
