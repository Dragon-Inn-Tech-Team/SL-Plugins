using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Choas.Features.AdminToyToys.ToyAttach;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;
using Utils.NonAllocLINQ;

namespace Choas.Features.AdminToyToys;

    
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AttachToyParent : ParentCommand, ICustomCommand
{
    public override string Command => "attachtoy";
    
    public override string[] Aliases => null;

    public string[] Usage => ["net id/subcommand"];
    public PlayerPermissions? Permission => PlayerPermissions.FacilityManagement;
    public string PermissionString => string.Empty;
    public bool RequirePlayerSender => false;
    public bool SanitizeResponse => false;
    public override string Description => "Attaches an admin toy to an object based on the sub command.";

    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new AttachToyPlayer());
        RegisterCommand(new AttachToyToy());
    }
    
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count == 0)
        {
            response = $"Please provide a valid subcommand\n{string.Join("/", this.Commands.Select(r => r.Value.Command))}\nOr a NetID to detach it.";
            return false;
        }
        
        if (!sender.CanRun(this, arguments, out response, out _, out _)) return false;

        if (!(uint.TryParse(arguments.At(0), out var toyid) &&
              NetworkUtils.SpawnedNetIds.TryGetValue(toyid, out var toyNetID)))
        {
            response = "Could not find toy";
            return false;
        }
        
        toyNetID.gameObject.transform.SetParent(null);
        
        response = "Toy is now an orphan";
        return true;
    }

}