using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using Mirror;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;

namespace Choas.Features.Jackenstein;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SpawnPumpkin : ICustomCommand
{
    public string Command => "pumpkin";

    public string[] Aliases => null;

    public string Description => "Spawns Jackenstein's pumpkin.";

    public string[] Usage => ["%player%"];

    public PlayerPermissions? Permission => PlayerPermissions.FacilityManagement;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out var plrs, out _)) return false;

        var pumpkin = YOUR_TAKING_TOO_LONG.CreateYOUR_TAKING_TOO_LONG(plrs.First().Camera.position + plrs.First().Camera.forward.NormalizeIgnoreY()*10, Quaternion.identity);

        float duration = 20f;
        if (arguments.Count >= 2 && float.TryParse(arguments.At(1), out var d)) 
            duration = d;
        
        bool lethal = arguments.Count >= 3 && arguments.At(2).Equals("lethal");
        
        pumpkin.StartChase(plrs, duration, lethal);
        
        response = $"your taking too long or something (netID: {pumpkin.Pumpkin.Base.netId})";
        return true;
    }
}