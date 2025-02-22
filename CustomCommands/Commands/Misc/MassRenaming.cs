using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.NonAllocLINQ;

namespace CustomCommands.Commands.Misc
{
	public class MassNaming
	{
		[CommandHandler(typeof(RemoteAdminCommandHandler))]
		public class RenameAll : ICustomCommand
		{
			public string Command => "renameall";

			public string[] Aliases { get; } = { };

			public string Description => "Renames every player based on an input. Run this command with no arugments to see more info.";

			public string[] Usage { get; } = { "Nickname" };

			public PlayerPermissions? Permission => PlayerPermissions.PlayersManagement;
			public string PermissionString => string.Empty;

			public bool RequirePlayerSender => false;

			public bool SanitizeResponse => false;

            private const string unname = "Use the command \"unnameall\" to reset everyone back to their normal names";

            private const string parsing = "There are a few special tags that get replaced which you can use in the nickname string:\n" +
                                            "   - {b}: Gets replaced with the player's default name\n" +
                                            "   - {n}: A number that counts up for each player renamed\n" +
                                            "   - {a}: A letter of the alphabet that increases for each player renamed\n" +
                                            "   - {r}: The player's role\n" +
                                            "   - {t}: The player's team";

			public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
			{
				try
				{
					if (arguments.Count == 0)
					{
						response = parsing + "\n\n" + unname;
						return true;
					}

					if (!sender.CanRun(this, arguments, out response, out var players, out _))
						return false;

					players = Player.GetPlayers();
					players.ShuffleList();

					string nick = string.Join(" ", arguments);

					int player_count = 0;
					for (int i = 0; i < players.Count; i++)
					{
						var plr = players.ElementAt(i);
						if (plr == null || plr.IsServer)
						{
							continue;
						}

						StringBuilder temp = new StringBuilder(nick);
						temp.Replace("{b}", plr.Nickname);
						temp.Replace("{n}", (player_count + 1).ToString());
						temp.Replace("{a}", ((char)(player_count + 'A')).ToString());
						temp.Replace("{t}", plr.Team.ToString());
						temp.Replace("{r}", plr.Role.ToString());						

                        plr.DisplayNickname = temp.ToString();
						player_count++;
					}

					response = $"Renamed {player_count} {(player_count != 1 ? "people" : "person")}";

					return true;
				}
				catch (Exception e)
				{
					response = "You borked something\n" + e.Message;
					return false;
				}
			}

			[CommandHandler(typeof(RemoteAdminCommandHandler))]
			public class UnnameAll : ICustomCommand
			{
				public string Command => "unnameall";

				public string[] Aliases { get; } = { };

				public string Description => "Resets every players' nickname";

				public string[] Usage { get; } = { };

				public PlayerPermissions? Permission => PlayerPermissions.PlayersManagement;
				public string PermissionString => string.Empty;

				public bool RequirePlayerSender => false;

				public bool SanitizeResponse => false;

				public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
				{
					if (!sender.CanRun(this, arguments, out response, out var players, out _))
						return false;

					players = Player.GetPlayers();

					int c = 0;
					foreach (Player plr in players)
					{
						plr.DisplayNickname = "";
						c++;
					}

					response = $"Reset the nicknames of {c} players";

					return true;
				}
			}
		}
	}
}
