using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Extensions = RedRightHand.Core.Extensions;

namespace ExternalQuery
{
	public class CustomCommandHandler
	{
		public static string BanCommand(string cmd)
		{
			try
			{
				string[] arg = cmd.Split(' ');

				string command = arg[0];
				string searchvariable = string.Empty;
				TimeSpan duration;
				string durationString = string.Empty;
				string reason = string.Empty;
				Player player = null;

				if (arg.Count() < 4 || arg[1].Length < 1)
					return $"{arg[0]} [UserID/IP] [Duration] [Reason]";

				//Forces the search variable to the front of the array
				arg = arg.Skip(1).ToArray();

				if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
				{
					string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

					searchvariable = result;

					arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
				}
				else
				{
					searchvariable = arg[0];

					arg = arg.Skip(1).ToArray();
				}

				durationString = arg[0];
				var chars = durationString.Where(Char.IsLetter).ToArray();
				if (chars.Length < 1 || !int.TryParse(new string(durationString.Where(Char.IsDigit).ToArray()), out int amount) || !Extensions.ValidDurationUnits.Contains(chars[0]) || amount < 1)
					return "Invalid duration provided";

				GetPlayer(searchvariable, out player);

				duration = Extensions.GetBanDuration(chars[0], amount);
				arg = arg.Skip(1).ToArray();
				reason = string.Join(" ", arg);

				if (player != null)
				{

					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = player.Nickname,
						Id = player.UserId,
						Issuer = "SERVER (EXTERNAL QUERY)",
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, BanHandler.BanType.UserId);

					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = player.Nickname,
						Id = player.IpAddress,
						Issuer = "SERVER (EXTERNAL QUERY)",
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, BanHandler.BanType.IP);

					player.Disconnect($"You have been banned by the server staff\nReason: " + reason);

					return $"{player.Nickname} ({player.UserId}) was banned for {durationString} with reason: {reason}";
				}
				else
				{
					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = "Offline player",
						Id = searchvariable,
						Issuer = "SERVER (EXTERNAL QUERY)",
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, searchvariable.Contains('@') ? BanHandler.BanType.UserId : BanHandler.BanType.IP);
					return $"{searchvariable} was banned for {durationString} with reason: {reason}!";
				}
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		public static string KickCommand(string cmd)
		{
			string[] arg = cmd.Split(' ');

			if (arg.Count() < 3 || arg[1].Length < 1)
				return $"{arg[0]} [UserID/IP] [Reason]";

			string searchvariable = string.Empty;

			//Forces the search variable to the front of the array
			arg = arg.Skip(1).ToArray();

			if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
			{
				string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

				searchvariable = result;

				arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
			}
			else
			{
				searchvariable = arg[0];

				arg = arg.Skip(1).ToArray();
			}

			if (!GetPlayer(searchvariable, out Player player))
				return $"Unable to find player {searchvariable.Replace("", "\\")}";

			string reason = string.Join(" ", arg);

			player.Disconnect($"You have been kicked by the server staff\nReason: " + reason);

			return $"{player.Nickname} ({player.UserId}) was kicked with reason: {reason}";
		}
		public static string UnbanCommand(string cmd)
		{
			string[] arg = cmd.Split(' ');

			if (arg.Count() < 2) return $"{arg[0]} [UserID/Ip]";

			bool validUID = arg[1].Contains('@');
			bool validIP = IPAddress.TryParse(arg[1], out IPAddress ip);

			BanDetails details;

			if (!validIP && !validUID)
				return $"Invalid UserID or IP given";

			if (validUID)
				details = BanHandler.QueryBan(arg[1], null).Key;
			else
				details = BanHandler.QueryBan(null, arg[1]).Value;

			if (details == null)
				return $"No ban found for {arg[1]}.\nMake sure you have typed it correctly, and that it has the @domain prefix if it's a UserID";

			BanHandler.RemoveBan(arg[1], (validUID ? BanHandler.BanType.UserId : BanHandler.BanType.IP));

			return $"{arg[1]} has been unbanned.";
		}


		private static bool GetPlayer(string SearchParameter, out Player Plr)
		{
			Plr = null;

			var plrs = Player.GetPlayers();
			IEnumerable<Player> posPlrs;

			if (SearchParameter.Contains('@'))
			{
				posPlrs = plrs.Where(p => p.UserId == SearchParameter).ToArray();

				if (posPlrs.Any())
				{
					Plr = posPlrs.First();
					return true;
				}
			}
			else if (IPAddress.TryParse(SearchParameter, out IPAddress IP))
			{
				posPlrs = plrs.Where(p => p.IpAddress == SearchParameter).ToArray();

				if (posPlrs.Any())
				{
					Plr = posPlrs.First();
					return true;
				}
			}

			posPlrs = plrs.Where(p => p.Nickname.ToLowerInvariant() == SearchParameter.ToLowerInvariant()).ToArray();

			if (posPlrs.Any())
			{
				Plr = posPlrs.First();
				return true;
			}

			return false;
		}
	}
}
