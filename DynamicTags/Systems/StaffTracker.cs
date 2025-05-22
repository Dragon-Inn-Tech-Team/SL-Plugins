using MEC;
using Newtonsoft.Json;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Extensions = RedRightHand.Core.Extensions;

namespace DynamicTags.Systems
{
	public class StaffTracker
	{
		//[PluginEvent]
		public void OnPlayerPreauth(PlayerPreauthEvent args)
		{
			Timing.RunCoroutine(CheckPreauth(args));
		}

		public IEnumerator<float> CheckPreauth(PlayerPreauthEvent args)
		{
			try
			{
				var details = new PlayerDetails
				{
					UserId = args.UserId,
					Address = args.IpAddress,
					ServerAddress = Server.ServerIpAddress,
					ServerPort = Server.Port.ToString()
				};

				var httpRM = Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerpreauth", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json")).Result;
				var response = JsonConvert.DeserializeObject<APIResponse>(httpRM.Content.ReadAsStringAsync().Result);

				Log.Info($"{response.Action} | {response.ReasonPlayer} | {response.ReasonPlayer}");

				args.ConnectionRequest.RejectForce();
			}
			catch (Exception e)
			{
				Log.Error($"Error during PlayerPreauthEvent: " + e.ToString());
			}

			yield return 0f;
		}

		[PluginEvent(ServerEventType.PlayerJoined)]
		public void OnPlayerJoin(PlayerJoinedEvent args)
		{
			try
			{
				var details = new PlayerDetails
				{
					UserId = args.Player.UserId,
					UserName = args.Player.Nickname,
					Address = args.Player.IpAddress,
					ServerAddress = Server.ServerIpAddress,
					ServerPort = Server.Port.ToString()
				};

				Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerjoin", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
			}
			catch (Exception e)
			{
				Log.Error($"Error during PlayerJoinedEvent: " + e.ToString());
			}
		}

		[PluginEvent(ServerEventType.PlayerLeft)]
		public void OnPlayerLeave(PlayerLeftEvent args)
		{
			try
			{
				var details = new PlayerDetails
				{
					UserId = args.Player.UserId,
					UserName = args.Player.Nickname,
					Address = args.Player.IpAddress,
					ServerAddress = Server.ServerIpAddress,
					ServerPort = Server.Port.ToString()
				};

				Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerleave", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Log.Error($"Error during PlayerLeftEvent: " + e.ToString());
			}
		}


		[PluginEvent(ServerEventType.PlayerBanned)]
		public void OnPlayerBanned(PlayerBannedEvent args)
		{
			try
			{
				var details = new PlayerBanDetails
				{
					PlayerName = args.Player.Nickname.Replace(':', ' '),
					PlayerID = args.Player.UserId,
					PlayerAddress = args.Player.IpAddress,
					AdminName = args.Issuer.Nickname.Replace(':', ' '),
					AdminID = args.Issuer.UserId,
					Duration = (args.Duration / 60).ToString(),
					Reason = string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason
				};

				Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerban", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Log.Error($"Error during PlayerBannedEvent: " + e.ToString());
			}
		}

		[PluginEvent(ServerEventType.PlayerKicked)]
		public void OnPlayerKicked(PlayerKickedEvent args)
		{
			try
			{
				PlayerBanDetails details;

				if (args.Issuer is PlayerCommandSender pCS)
				{
					var admin = Player.Get(pCS.PlayerId);

					details = new PlayerBanDetails
					{
						PlayerName = args.Player.Nickname.Replace(':', ' '),
						PlayerID = args.Player.UserId,
						PlayerAddress = args.Player.IpAddress,
						AdminName = admin.Nickname.Replace(':', ' '),
						AdminID = admin.UserId,
						Duration = "0",
						Reason = string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason
					};
				}
				else
				{
					details = new PlayerBanDetails
					{
						PlayerName = args.Player.Nickname.Replace(':', ' '),
						PlayerID = args.Player.UserId,
						PlayerAddress = args.Player.IpAddress,
						AdminName = "SERVER",
						AdminID = "server",
						Duration = "0",
						Reason = string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason
					};

				}
				Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerkick", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Log.Error($"Error during PlayerKickedEvent: " + e.ToString());
			}
		}

		[PluginEvent(ServerEventType.PlayerMuted)]
		public void PlayerMuteEvent(PlayerMutedEvent args)
		{
			try
			{
				var details = new PlayerBanDetails
				{
					PlayerName = args.Player.Nickname.Replace(':', ' '),
					PlayerID = args.Player.UserId,
					PlayerAddress = args.Player.IpAddress,
					AdminName = args.Issuer.Nickname.Replace(':', ' '),
					AdminID = args.Issuer.UserId,
					Duration = args.IsIntercom.ToString(),
					Reason = "No reason provided"
				};

				Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playermute", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Log.Error($"Error during PlayerMutedEvent: " + e.ToString());
			}
		}

		[PluginEvent(ServerEventType.PlayerUnmuted)]
		public void PlayerUnmuteEvent(PlayerUnmutedEvent args)
		{
			try
			{
				var details = new PlayerBanDetails
				{
					PlayerName = args.Player.Nickname.Replace(':', ' '),
					PlayerID = args.Player.UserId,
					PlayerAddress = args.Player.IpAddress,
					AdminName = args.Issuer.Nickname.Replace(':', ' '),
					AdminID = args.Issuer.UserId,
					Duration = args.IsIntercom.ToString(),
					Reason = "No reason provided"
				};

				Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerunmute", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Log.Error($"Error during PlayerUnmutedEvent: " + e.ToString());
			}
		}
	}
}
