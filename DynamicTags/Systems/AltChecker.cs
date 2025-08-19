using HarmonyLib;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Newtonsoft.Json;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTags.Systems
{
	[HarmonyPatch(typeof(BanHandler))]
	[HarmonyPatch("QueryBan")]
	public class AltChecker
	{
		[HarmonyPostfix]
		public static void PostFix(ref KeyValuePair<BanDetails, BanDetails> __result, ref string userId, ref string ip)
		{
			try
			{
				Logger.Info($"AAAAAAAAAA {__result.Key == null} {__result.Value == null}");

				if (__result.Key != null && __result.Value == null)
				{
					SendToDiscord($"***POSSIBLE BAN BYPASS ATTEMPT***" +
						$"\nTrigger Account: {userId} from ~~||{ip}||~~" +
						$"\nBan Type: UserId (IP address not banned)" +
						$"\nReason: {__result.Key.Reason}" +
						$"\nIssued: <t:{__result.Key.IssuanceTime}:f>" +
						$"\nExpires: <t:{__result.Key.Expires}:f>");
				}
				else if (__result.Key == null && __result.Value != null)
				{
					SendToDiscord($"***POSSIBLE BAN BYPASS ATTEMPT***" +
						$"\nTrigger Account: {userId} from ~~||{ip}||~~" +
						$"\nBan Type: IP Address (UserID not banned)" +
						$"\nReason: {__result.Value.Reason}" +
						$"\nIssued: <t:{new DateTimeOffset(new DateTime(__result.Value.IssuanceTime)).ToUnixTimeSeconds()}:f>" +
						$"\nExpires: <t:{new DateTimeOffset(new DateTime(__result.Value.Expires)).ToUnixTimeSeconds()}:f>");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		public static void SendToDiscord(string content)
		{
			try
			{
				var webhook = new Webhook(content, "SL Ban Bypass Tracker");

				RedRightHand.Extensions.Post(DynamicTagsPlugin.Config.CommandTrackingWebhook, new StringContent(JsonConvert.SerializeObject(webhook), Encoding.UTF8, "application/json"));
			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerJoinedEvent: " + e.ToString());
			}
		}
	}

	public class Webhook
	{
		public string content;
		public string username;
		public string avatar_url;
		public bool tts = false;

		public Webhook(string content, string username)
		{
			this.content = content;
			this.username = username;
		}
	}
}
