using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Choas.Features.AdminToyToys.ToyLookAt;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;

namespace Choas.Features.AdminToyToys;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class MakeToyLookAtParent : ParentCommand
{
    public override string Command => "toystare";
    
    public override string[] Aliases => null;

    public override string Description => "Makes an admin toy stare at an object based on the sub command";

    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new ToyLookAtPlayer());
        RegisterCommand(new ToyLookAtPos());
        RegisterCommand(new ToyLookAtToy());
    }
    
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = $"Please provide a valid subcommand\n{string.Join("/", this.Commands.Select(r => r.Value.Command))}";
        return false;
    }

}