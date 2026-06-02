using CommandSystem;
using LabApi.Features.Wrappers;
using NetworkManagerUtils.Dummies;
using RedRightHand;
using RedRightHand.Commands;
using System;
using System.Linq;

namespace CustomCommands.Features.TestingFeatures.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Fill : ICustomCommand
	{
		public string Command => "dummyfill";

		public string[] Aliases => null;

		public string Description => "Fills the server with dummy players";

		public string[] Usage { get; } = { "name" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyf";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (!CustomCommandsPlugin.Config.EnableDebugTests)
			{
				response = "This command is disabled";
				return false;
			}

			var dumsToMake = Server.MaxPlayers - Server.PlayerCount;

			for (int i = 0; i < dumsToMake; i++)
			{
				DummyUtils.SpawnDummy(arguments.ElementAt(0) + i);
			}

			response = $"Server filled with {dumsToMake} dummies";

			return true;
		}
	}
}
