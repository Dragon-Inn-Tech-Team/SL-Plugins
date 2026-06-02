using CommandSystem;
using CustomCommands.Features.DoorLocking.Commands;
using System;
using System.Linq;

namespace CustomCommands.Commands.Player
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PlayerParent : ParentCommand
	{
		public override string Command => "player";

		public override string[] Aliases { get; } = { "plr" };

		public override string Description => "Various commands targetted at players";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Drop());
			RegisterCommand(new Explode());
			RegisterCommand(new Hint());
			RegisterCommand(new Pocket());
			RegisterCommand(new SendTo());
			RegisterCommand(new TeleportToCoords());
			RegisterCommand(new Tower2());
			RegisterCommand(new Trip());
			RegisterCommand(new Doors());
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = $"Please provide a valid subcommand\n{string.Join("/", this.Commands.Select(r => r.Value.Command))}";
			return false;
		}
	}
}
