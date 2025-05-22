using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class List : ICommand
	{
		public string Command => "scplist";

		public string[] Aliases { get; } = { "slist" };
		public string Description => "Lists all current SCPs";

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP())
			{
				var plr = Player.Get(pSender.ReferenceHub);

				var scps = Player.GetPlayers().Where(r => r.IsSCP);

				if (!scps.Any())
				{
					response = "There are no other SCPs";
					return false;
				}

				List<string> scpString = new List<string>();
				foreach (var a in scps)
				{
					scpString.Add(a.Role.ToString().ToLower().Replace("scp", string.Empty));
				}

				response = $"Current SCPs: {string.Join(", ", scpString)}";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
