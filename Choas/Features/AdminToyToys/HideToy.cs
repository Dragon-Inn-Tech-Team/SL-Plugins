using System;
using CommandSystem;
using Mirror;
using RedRightHand;
using RedRightHand.Commands;
using Utils.Networking;

namespace Choas.Features.AdminToyToys;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class HideToy : ICustomCommand
{
    public string Command => "hidetoy";

    public string[] Aliases => null;

    public string Description => "Hides an admin toy for some players.";

    public string[] Usage => ["net id", "%player%"];

    public PlayerPermissions? Permission => PlayerPermissions.FacilityManagement;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out var plrs, out _)) return false;
        
        if (!(uint.TryParse(arguments.At(0), out var toyid) &&
              NetworkUtils.SpawnedNetIds.TryGetValue(toyid, out var toyNetID)))
        {
            response = "Could not find toy";
            return false;
        }
        
        foreach (var plr in plrs)
        {
            plr.Connection.Send(new ObjectHideMessage { netId = toyid });
        }
        
        response = "THe thingy hgas been hidden";
        return true;
    }
}