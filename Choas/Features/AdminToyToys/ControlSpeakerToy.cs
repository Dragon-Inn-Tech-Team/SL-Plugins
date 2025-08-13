using System;
using System.Linq;
using Choas.Features.AdminToyToys.SpeakerToyCommands;
using CommandSystem;

namespace Choas.Features.AdminToyToys;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ControlSpeakerToy : ParentCommand
{
    public override string Command => "speakertoy";
    public override string[] Aliases => null;
    public override string Description => "Various commands to control a speaker toy.";
    
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new Info());
        RegisterCommand(new Load());
        RegisterCommand(new Loop());
        RegisterCommand(new Pause());
        RegisterCommand(new SetControllerID());
        RegisterCommand(new SetDistance());
        RegisterCommand(new SetSpatial());
        RegisterCommand(new Skip());
        RegisterCommand(new Track());
        RegisterCommand(new Unpause());
        RegisterCommand(new Volume());
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = $"Please provide a valid subcommand:\n{string.Join("\n", this.Commands.Select(r => r.Value.Command + ":\t" + r.Value.Description))}";
        return false;
    }

}