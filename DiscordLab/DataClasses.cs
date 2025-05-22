using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordLab
{
	/// <summary>
	/// Base class for all msg classes (Used for sending messages to the bot).
	/// </summary>
	public class MsgBase
	{
		public MsgBase() { }


		public MessageType Type;
	}

	/// <summary>
	/// <see cref="MsgBase"/> class for sending string messages to the bot (Such as "Hello, World!").
	/// </summary>
	public class Msg : MsgBase
	{
		public Msg(string message)
		{
			Type = MessageType.msg;
			Message = BotSocketConnection.MsgRgx.Replace(message, string.Empty);
		}

		public string Message;
	}
	/// <summary>
	/// <see cref="MsgBase"/> class for responding to commands recieved by the bot.
	/// </summary>
	public class Command : MsgBase
	{
		public Command(string channelID, string staffID, string msg)
		{
			ChannelID = channelID;
			StaffID = staffID;
			CommandMessage = msg;
			Type = MessageType.cmdmsg;
		}

		public string CommandMessage;
		public string ChannelID;
		public string StaffID;
	}
	/// <summary>
	/// <see cref="MsgBase"/> class for responding to the bot's request for the player list.
	/// </summary>
	public class PlayerList : MsgBase
	{
		public PlayerList(string channelId)
		{
			Type = MessageType.plist;
			ChannelID = channelId;

			if (RoundRestarting.RoundRestart.IsRoundRestarting)
			{
				PlayerNames = "**The round has recently restarted, and players are rejoining**";
			}
			else
			{
				var players = Player.GetPlayers();

				if (Player.Count < 1)
				{
					if ((DateTime.Now - Events.RoundEndTime).TotalSeconds < 45)
						PlayerNames = "**The round has recently restarted, and players are rejoining**";
					else
						PlayerNames = "**No online players**";
				}
				else
				{
					int DntCount = 0;
					List<string> plrNames = new List<string>();

					foreach (Player plr in players)
					{
						if (plr.IsServer)
							continue;
						if (!plr.DoNotTrack)
							plrNames.Add(BotSocketConnection.NameRgx.Replace(plr.Nickname, string.Empty));
						else
							DntCount++;
					}

					if (DntCount > 0)
						plrNames.Add($"{(Player.Count > 1 ? "and " : "")}{DntCount}{(Player.Count > 1 ? " other" : "")} DNT user{(DntCount > 1 ? "s" : "")}");

					PlayerNames = $"{string.Join(", ", plrNames)}";
				}

				CurrentPlayers = Player.Count + "/" + Server.MaxPlayers;
			}
		}

		public string PlayerNames;
		public string ChannelID;
		public string CurrentPlayers;
	}

	/// <summary>
	/// <see cref="MsgBase"/> class for keeping the bot's player count status updated.
	/// </summary>
	public class Status : MsgBase
	{
		public Status()
		{
			Type = MessageType.supdate;
			CurrentPlayers = Player.Count + "/" + Server.MaxPlayers;
			LastUpdate = DateTime.Now;
		}

		public string CurrentPlayers { get; }
		internal DateTime LastUpdate { get; }
	}

	/// <summary>
	/// <see cref="MsgBase"/> class for keeping the connection between the bot and server alive.
	/// </summary>
	public class KeepAlive : MsgBase
	{
		public KeepAlive()
		{
			Type = MessageType.keepalive;
		}
	}

	public enum MessageType
	{
		/// <summary>
		/// <see cref="Msg"/> message
		/// </summary>
		msg = 0,
		/// <summary>
		/// <see cref="Command"/> message.
		/// </summary>
		cmdmsg = 1,
		/// <summary>
		/// <see cref="PlayerList"/> message.
		/// </summary>
		plist = 2,
		/// <summary>
		/// <see cref="Status"/> message.
		/// </summary>
		supdate = 3,
		/// <summary>
		/// <see cref="KeepAlive"/> message.
		/// </summary>
		keepalive = 4
	}
}
