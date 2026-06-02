using CommandSystem;
using System;
using System.Linq;

namespace CustomCommands.Commands.Check
{
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CheckParent : ParentCommand
	{
		public override string Command => "check";

		public override string[] Aliases => [];

		public override string Description => "Check info for an offline player";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Ban());
			RegisterCommand(new Mute());
		}

		public static CheckParent Create()
		{
			var cleanupCommand = new CheckParent();
			cleanupCommand.LoadGeneratedCommands();
			return cleanupCommand;
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = $"Please provide a valid subcommand\n{string.Join("/", this.Commands.Select(r => r.Value.Command))}";
			return false;
		}

	}
}
