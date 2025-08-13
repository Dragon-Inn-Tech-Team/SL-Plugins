using System;
using CommandSystem;
using Mirror;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;

namespace Choas.Features.PlayerCommands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class Jump : ICustomCommand
{
    public string Command => "jump";

    public string[] Aliases => null;

    public string Description => "Makes players jump.";

    public string[] Usage => ["%player%", "jump strength"];

    public PlayerPermissions? Permission => PlayerPermissions.Effects;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out var plrs, out _)) return false;

        if (!float.TryParse(arguments.At(1), out var strength))
        {
            response = "Could not parse strength.";
            return false;
        }
        
        foreach (var player in plrs)
        {
            player.Jump(strength);
        }
        
        response = $"Made {plrs.Count} player{(plrs.Count == 1 ? "" : "s")} jump.";
        return true;
    }
}