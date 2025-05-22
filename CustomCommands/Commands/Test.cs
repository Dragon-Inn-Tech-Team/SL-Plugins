using CommandSystem;
using RedRightHand.Core;
using RedRightHand.Core.Commands;
using System;

namespace CustomCommands.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class TestCommand : ICustomCommand
	{
		public string Command => "nevergonna";

		public string[] Aliases => null;

		public string Description => "Test command. Try it :)";

		public string[] Usage => new[] { "give"/*, "you", "up"*/ };

		public PlayerPermissions? Permission => null;

		public string PermissionString => "test";

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out var pSender))
				return false;

			int index = 0;

			response = $"Never gonna give you up,\nNever gonna let you down.\nNever gonna run around,\nAnd desert you.\nNever gonna make you cry,\nNever gonna say goodbye.\nNever gonna tell a lie,\nAnd hurt you.";
			return true;
		}
	}
}
