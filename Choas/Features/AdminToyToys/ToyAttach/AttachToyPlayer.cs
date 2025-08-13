using System;
using System.Linq;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using Utils.Networking;

namespace Choas.Features.AdminToyToys.ToyAttach;

[CommandHandler(typeof(AttachToyParent))]
public class AttachToyPlayer : ICustomCommand
{
    public string Command => "player";

    public string[] Aliases => null;

    public string Description => "Attaches a toy to a player.";

    public string[] Usage => ["net id", "%player%"];

    public PlayerPermissions? Permission => PlayerPermissions.FacilityManagement;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out var plrs, out _)) return false;

        var plr = plrs.First();

        if (!(uint.TryParse(arguments.At(0), out var toyid) &&
              NetworkUtils.SpawnedNetIds.TryGetValue(toyid, out var toyNetID)))
        {
            response = "Could not find toy.";
            return false;
        }
        
        toyNetID.gameObject.transform.SetParent(plr.GameObject.transform);
        
        response = "THe thingy hgas been adopted";
        return true;
    }
}