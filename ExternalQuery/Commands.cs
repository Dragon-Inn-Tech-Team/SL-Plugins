using CommandSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;

namespace ExternalQuery
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ListCommand : ICommand, IUsageProvider
	{
		public string Command => "list";

		public string[] Aliases { get; } = { "plist", "players" };

		public string Description => "Get a list of all players on the server";

		public string[] Usage { get; } = { };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Server.PlayerCount < 1)
			{
				response = "No players currently online";
				return true;
			}

			var plrStrs = new List<string>();

			foreach (var plr in Player.GetPlayers())
			{
				if (plr.IsServer)
					continue;

				plrStrs.Add($"[{plr.PlayerId}] - {plr.Nickname}");
			}

			response = $"[{Server.PlayerCount}/{Server.MaxPlayers}] Current players: \n{string.Join("\n", plrStrs)}";

			return true;
		}
	}
}
