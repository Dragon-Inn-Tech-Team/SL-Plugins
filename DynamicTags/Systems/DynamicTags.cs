using CommandSystem;
using Newtonsoft.Json;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using Extensions = RedRightHand.Core.Extensions;

namespace DynamicTags.Systems
{
	public class DynamicTags
	{
		[CommandHandler(typeof(RemoteAdminCommandHandler))]
		[CommandHandler(typeof(ClientCommandHandler))]
		public class DynamicTagCommand : ICommand
		{
			public string Command => "dynamictag";

			public string[] Aliases { get; } = { "dtag", "dt" };

			public string Description => "Shows your dynamic tag";

			public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
			{
				if (sender is PlayerCommandSender pSender)
				{
					if (Tags.ContainsKey(pSender.ReferenceHub.authManager.UserId))
					{
						TagData data = Tags[pSender.ReferenceHub.authManager.UserId];

						//This is to stop situations where users have locally assigned perms but gets overridden by NULL perms from the external server.
						if (!string.IsNullOrEmpty(data.Group))
						{
							pSender.ReferenceHub.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler().GetGroup(data.Group), true);
							pSender.ReferenceHub.serverRoles.RemoteAdmin = true;

							if (data.Perms != 0)
								pSender.ReferenceHub.serverRoles.Permissions = data.Perms;

						}

						pSender.ReferenceHub.serverRoles.SetText(data.Tag);
						pSender.ReferenceHub.serverRoles.SetColor(data.Colour);



						response = "Dynamic tag loaded: " + data.Tag;
						return true;
					}
					response = "You have no tag";
					return true;
				}

				response = "This command must be run as a player command";
				return false;
			}
		}

		[CommandHandler(typeof(RemoteAdminCommandHandler))]
		public class DynamicTagListCommand : ICommand
		{
			public string Command => "dynamictaglist";

			public string[] Aliases { get; } = { "dtaglist", "dtl" };

			public string Description => "Lists all dynamic tags";

			public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
			{
				if (sender is PlayerCommandSender pSender)
				{
					if (sender.CheckPermission(PlayerPermissions.PermissionsManagement))
					{
						List<string> tags = new List<string>();

						foreach (var tag in Tags)
						{
							tags.Add($"{tag.Key} | {tag.Value.Tag}");
						}

						response = string.Join("\n", tags);
						return true;
					}
					response = "You cannot run this command";
					return true;
				}

				response = "This command must be run as a player command";
				return false;
			}
		}

		[CommandHandler(typeof(RemoteAdminCommandHandler))]
		public class UpdateDynamicTagsCommand : ICommand
		{
			public string Command => "dynamictagupdate";

			public string[] Aliases { get; } = { "dtagupdate", "dtu" };

			public string Description => "Updates all dynamic tags";

			public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
			{
				if (sender is PlayerCommandSender pSender)
				{
					if (sender.CheckPermission(PlayerPermissions.PermissionsManagement))
					{
						UpdateTags(true);

						response = "Dynamic tags updated";
						return true;
					}
					response = "You cannot run this command";
					return true;
				}

				response = "This command must be run as a player command";
				return false;
			}
		}

		public static Dictionary<string, TagData> Tags = new Dictionary<string, TagData>();

		[PluginEvent(ServerEventType.WaitingForPlayers)]
		public async void OnWaitingForPlayers()
		{
			UpdateTags();
		}

		public static async void UpdateTags(bool ForceUpdate = false)
		{
			try
			{
				//Clears all previous tags held by the server (Prevents players from keeping tags when they have been removed from the external server).
				Tags.Clear();

				var response = await Extensions.Get(Plugin.Config.ApiEndpoint + "games/gettags");

				var tags = JsonConvert.DeserializeObject<TagData[]>(await response.Content.ReadAsStringAsync());

				foreach (var a in tags)
				{
					if (a.UserID.StartsWith("7656"))
						a.UserID = $"{a.UserID}@steam";
					else if (ulong.TryParse(a.UserID, out ulong result))
						a.UserID = $"{a.UserID}@discord";
					else
						a.UserID = $"{a.UserID}@northwood";

					//Adds the tags to the tag list.
					Tags.Add(a.UserID, a);
				}

				Log.Info($"{Tags.Count} tags loaded");

				foreach (var plr in Player.GetPlayers())
					if (Tags.ContainsKey(plr.UserId))
						SetDynamicTag(plr, Tags[plr.UserId]);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		[PluginEvent(ServerEventType.PlayerCheckReservedSlot)]
		public PlayerCheckReservedSlotCancellationData OnReservedSlotCheck(PlayerCheckReservedSlotEvent args)
		{
			if (args.HasReservedSlot)
				return PlayerCheckReservedSlotCancellationData.LeaveUnchanged();

			else if (args.Userid.ToLower().Contains("northwood") && Plugin.Config.AutomaticNorthwoodReservedSlot)
			{
				Log.Info($"Reserved slot bypass for {args.Userid} (Northwood ID detected)");
				return PlayerCheckReservedSlotCancellationData.BypassCheck();
			}
			else if (Tags.ContainsKey(args.Userid) && Tags[args.Userid].ReservedSlot)
			{
				Log.Info($"Reserved slot bypass for {args.Userid} (Dynamic Tag)");
				return PlayerCheckReservedSlotCancellationData.BypassCheck();
			}

			else return PlayerCheckReservedSlotCancellationData.LeaveUnchanged();
		}

		[PluginEvent(ServerEventType.PlayerJoined)]
		public void OnPlayerJoin(PlayerJoinedEvent args)
		{
			//Checks if the user has a tag
			if (Tags.ContainsKey(args.Player.UserId))
				SetDynamicTag(args.Player, Tags[args.Player.UserId]);

		}

		public static void SetDynamicTag(Player player, TagData data)
		{
			//This is to stop situations where users have locally assigned perms but gets overridden by NULL perms from the external server.
			if (!string.IsNullOrEmpty(data.Group))
				player.ReferenceHub.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler().GetGroup(data.Group), true);

			player.ReferenceHub.serverRoles.SetText(data.Tag);
			player.ReferenceHub.serverRoles.SetColor(data.Colour);

			if (data.Perms != 0)
				player.ReferenceHub.serverRoles.Permissions = data.Perms;

			player.SendConsoleMessage("Dynamic tag loaded: " + data.Tag);
			Log.Info($"Tag found for {player.UserId}: {data.Tag}");
		}
	}
}
