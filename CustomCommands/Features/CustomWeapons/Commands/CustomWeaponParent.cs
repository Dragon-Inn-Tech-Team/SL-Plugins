using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomWeapons.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CustomWeaponParent : ParentCommand
	{
		public override string Command => "customweapon";

		public override string[] Aliases => ["weapon", "cw", "customgun"];

		public override string Description => "Enable/Disable a custom weapon for a player";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Ball());
			RegisterCommand(new Capybara());
			RegisterCommand(new Flash());
			RegisterCommand(new Grenade());
			RegisterCommand(new Ragdoll());
			RegisterCommand(new Confetti());
		}

		public static CustomWeaponParent Create()
		{
			var cmd = new CustomWeaponParent();
			cmd.LoadGeneratedCommands();
			return cmd;
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = $"Please provide a valid subcommand\n{string.Join("/", this.Commands.Select(r => r.Value.Command))}";
			return false;
		}
	}
}
