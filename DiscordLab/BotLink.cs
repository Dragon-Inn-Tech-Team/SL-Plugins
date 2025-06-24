using Discord;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Extensions = RedRightHand.Extensions;

namespace DiscordLab
{
    public class BotLink
    {
		public static BotLink Instance { get; private set; }

		/// <summary>
		/// <see cref="Regex"/> used for cleaning messages (Prevents broken JSON strings and URLs/Discord invites).
		/// </summary>
		public static Regex MsgRgx = new("(.gg/)|(<@)|(http)|({)|(})|(<)|(>)|(\")|(\\[)|(\\])");
		/// <summary>
		/// <see cref="Regex"/> used for cleaning player names (Prevents broken JSON strings and formatting).
		/// </summary>
		public static Regex NameRgx = new("(\\*)|(_)|({)|(})|(@)|(<)|(>)|(\")|(\\[)|(\\])");

		/// <summary>
		/// <see cref="System.Net.Sockets.Socket"/> connection used to communicate between the bot and server.
		/// </summary>
		internal Socket Socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		/// <summary>
		/// <see cref="IPAddress"/> used for the connection to the bot.
		/// </summary>
		[Obsolete]
		public IPAddress Address;

#pragma warning disable IDE0052 // Remove unread private members
		private readonly Timer StatusTimer;
		private readonly Timer KeepAliveTimer;
		private DateTime _lastMsg;
#pragma warning restore IDE0052 // Remove unread private members

		private string BotAddress => $"{DiscordLabPlugin.Config.BotAddress}:{DiscordLabPlugin.Config.BotPort}";

		public BotLink()
		{
			try
			{
				Instance = this;
				StatusTimer = new Timer(t1 => StatusUpdate(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
				KeepAliveTimer = new Timer(t2 => KeepConnectionAlive(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

				OpenConnection();

				new Thread(() =>
				{
					BotListener();
				}).Start();

			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}

		private Status _lastStatus = null;
		/// <summary>
		/// Updates the status for the bot.
		/// </summary>
		public void StatusUpdate(bool force = false)
		{
			//Log.Info($"Updating Status!");

			if (force || (_lastStatus == null || _lastStatus.CurrentPlayers != (Player.Count + "/" + Server.MaxPlayers)))
			{
				var msg = new Status();
				SendMessage(msg);
				_lastStatus = msg;
			}
		}


		public void SendMessage(string msgContent) => SendMessage(new Msg(msgContent));

		/// <summary>
		/// Sends a message to the bot.
		/// </summary>
		/// <param name="message"><see cref="MsgBase"/></param>
		public void SendMessage(MsgBase message)
		{
			if (!Socket.Connected && !OpenConnection())
				return;

			var msg = JsonConvert.SerializeObject(message);

			try
			{
				Socket.Send(Encoding.UTF8.GetBytes(msg));
				_lastMsg = DateTime.Now;
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}
		}



		private void BotListener()
		{
			while (2 > 1)
			{
				if (Socket.Connected)
				{
					byte[] data = new byte[8192];
					int dataLength = Socket.Receive(data);

					string incomingData = Encoding.UTF8.GetString(data, 0, dataLength);

					//Logger.Info(incomingData);

					List<string> messages = new(incomingData.Split('\n'));

					foreach (string message in messages)
					{
						if (!string.IsNullOrEmpty(message))
						{
							JObject jObj = JsonConvert.DeserializeObject<JObject>(message);

							if (jObj["Type"].ToString().ToLower() == "plist")
							{
								var msg = new PlayerList(jObj["channel"].ToString());

								SendMessage(msg);

								StatusUpdate(true);
							}
							else if (jObj["Type"].ToString().ToLower() == "cmd")
								Commands(jObj);
						}
					}
				}
				Thread.Sleep(50);
			}
		}

		#region Connection Management



		private bool OpenConnection()
		{
			try
			{
				ConnectSocket();
			}
			catch (Exception)
			{
				try
				{
					Logger.Warn($"Bot connection failed. Resetting conection...");
					Socket.Disconnect(false);

					ConnectSocket();
				}
				catch (Exception e)
				{
					Logger.Error($"{DiscordLabPlugin.Config.BotAddress}:{DiscordLabPlugin.Config.BotPort} Unable to connect with the bot.\n{e}");
					return false;
				}
			}
			return true;
		}
		private void ConnectSocket()
		{
			Socket.Connect(DiscordLabPlugin.Config.BotAddress, DiscordLabPlugin.Config.BotPort);
			Logger.Info($"Bot connection established: {BotAddress}");
		}

		/// <summary>
		/// Prevents the bot's connection from closing.
		/// </summary>
		private void KeepConnectionAlive()
		{
			//Log.Info($"Keep Alive!");
			SendMessage(new KeepAlive());
		}

		#endregion

		#region Commands

		private void Commands(JObject jObj)
		{
			string[] command = jObj["Message"].ToString().Split(' ').Skip(1).ToArray();
			Logger.Info($"Remote command ran by {jObj["StaffID"]}: {command[0].ToLower()}");
			string response;

			try
			{
				response = command[0].ToLower() switch
				{
					"ban" or "rban" or "remoteban" => BanCommand(command, jObj),
					"kick" or "rkick" or "remotekick" => KickCommand(command, jObj),
					"unban" or "runban" or "remoteunban" => UnbanCommand(command, jObj),
					_ => "Unknown command",
				};
				SendMessage(new Command(jObj["channel"].ToString(), jObj["StaffID"].ToString(), response));
			}
			catch (Exception e)
			{
				Logger.Error($"Error running remote command: {e}\nCommand Info:{jObj["StaffID"]}: {command[0].ToLower()}");
			}
		}

		private string BanCommand(string[] arg, JObject jObject)
		{
			try
			{
				string command = arg[0];
				string searchvariable = string.Empty;
				TimeSpan duration;
				string durationString = string.Empty;
				string reason = string.Empty;

				if (arg.Count() < 4 || arg[1].Length < 1)
					return $"```{arg[0]} [UserID/IP] [Duration] [Reason]```";

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
					return "```diff\n- Invalid duration provided```";

				GetPlayer(searchvariable, out Player player);

				//foreach (var a in jObject)
				//{
				//	Logger.Info($"{a.Key} | {a.Value}");
				//}

				duration = Extensions.GetBanDuration(chars[0], amount);
				arg = arg.Skip(1).ToArray();
				reason = string.Join(" ", arg);

				if (player != null)
				{
					player.Disconnect($"You have been banned by the server staff\nReason: " + reason);

					BanHandler.IssueBan(new BanDetails
					{
						Id = player.UserId,
						IssuanceTime = TimeBehaviour.CurrentTimestamp(),
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Issuer = jObject["StaffID"].ToString(),
						OriginalName = player.Nickname,
						Reason = reason
					}, BanHandler.BanType.UserId);

					BanHandler.IssueBan(new BanDetails
					{
						Id = player.IpAddress,
						IssuanceTime = TimeBehaviour.CurrentTimestamp(),
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Issuer = jObject["StaffID"].ToString(),
						OriginalName = player.Nickname,
						Reason = reason
					}, BanHandler.BanType.IP);

					//Server.BanPlayer(player, reason, DateTime.UtcNow.Add(duration).Ticks);


					return $"`{player.Nickname} ({player.UserId})` was banned for {durationString} with reason: {reason}";
				}
				else
				{
					if (searchvariable.Contains('@'))
						BanHandler.IssueBan(new BanDetails
						{
							Id = searchvariable,
							IssuanceTime = TimeBehaviour.CurrentTimestamp(),
							Expires = DateTime.UtcNow.Add(duration).Ticks,
							Issuer = jObject["StaffID"].ToString(),
							OriginalName = "Offline Player",
							Reason = reason
						}, BanHandler.BanType.UserId);
					else
						BanHandler.IssueBan(new BanDetails
						{
							Id = searchvariable,
							IssuanceTime = TimeBehaviour.CurrentTimestamp(),
							Expires = DateTime.UtcNow.Add(duration).Ticks,
							Issuer = jObject["StaffID"].ToString(),
							OriginalName = "Offline Player",
							Reason = reason
						}, BanHandler.BanType.IP);

					return $"`{searchvariable}` was banned for {durationString} with reason: {reason}!";
				}
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		private string KickCommand(string[] arg, JObject jObject)
		{
			if (arg.Count() < 3 || arg[1].Length < 1)
				return $"```{arg[0]} [UserID/IP] [Reason]```";


			//Forces the search variable to the front of the array
			arg = arg.Skip(1).ToArray();


			string searchvariable;
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
				return $"Unable to find player `{searchvariable.Replace("`", "\\`")}`";

			string reason = string.Join(" ", arg);

			player.Disconnect($"You have been kicked by the server staff\nReason: " + reason);

			return $"`{player.Nickname} ({player.UserId})` was kicked with reason: {reason}";
		}
		private string UnbanCommand(string[] arg, JObject jObject)
		{
			if (arg.Count() < 2) return $"```{arg[0]} [UserID/Ip]```";

			bool validUID = arg[1].Contains('@');

			bool ban = Server.IsPlayerBanned(arg[1]);

			if (ban)
			{
				if (validUID)
					Server.UnbanUserId(arg[1]);
				else
					Server.UnbanIpAddress(arg[1]);

				return $"`{arg[1]}` has been unbanned.";
			}
			return $"No ban found for `{arg[1]}`.\nMake sure you have typed it correctly, and that it has the @domain prefix if it's a UserID";
		}

		#endregion


		private bool GetPlayer(string SearchParameter, out Player Plr)
		{
			Plr = null;

			var plrs = Player.List;
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

		public static void AddDiscordLog(string args)
		{
			AddLog?.Invoke(args);
		}
		public static event Action<string> AddLog;

		internal void LogAddedByPlugin(string obj)
		{
			Logger.Info($"Incoming message from plugins: {obj}");
			BotLink.Instance.SendMessage(obj);
		}
	}
}
