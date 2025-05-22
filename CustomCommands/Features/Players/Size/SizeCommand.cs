using CommandSystem;
using Mirror;
using PluginAPI.Core;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;
using System.Reflection;

namespace CustomCommands.Features.Players.Size
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SizeCommand : ICustomCommand
	{
		public string Command => "size";

		public string[] Aliases { get; } = { "scale" };
		public string Description => "Modify the size of a specified player";
		public string[] Usage { get; } = { "%player%", "x", "y", "z" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.size";
		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			if (!float.TryParse(arguments.Array[2], out float x) || !float.TryParse(arguments.Array[3], out float y) || !float.TryParse(arguments.Array[4], out float z))
			{
				response = "Valid scale not provided";
				return false;
			}

			var svrPlrs = Server.GetPlayers();

			foreach (var p in players)
			{
				var nId = p.ReferenceHub.networkIdentity;
				p.ReferenceHub.gameObject.transform.localScale = new UnityEngine.Vector3(1 * x, 1 * y, 1 * z);

				foreach (var player in svrPlrs)
				{
					NetworkConnection nConn = player.ReferenceHub.connectionToClient;

					typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { nId, nConn });
				}
			}

			response = $"Scale of {players.Count} {(players.Count != 1 ? "players" : "player")} has been set to {x}, {y}, {z}";
			return true;
		}
	}
}
