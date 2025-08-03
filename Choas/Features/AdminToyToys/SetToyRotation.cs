using System;
using System.Linq;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;

namespace Choas.Features.AdminToyToys;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetToyRotation : ICustomCommand
{
    public string Command => "toyrot";

    public string[] Aliases => null;

    public string Description => "Sets an admin toy's rotation to the specified (local) euler angles.";

    public string[] Usage => ["net id", "x", "y", "z"];

    public PlayerPermissions? Permission => PlayerPermissions.FacilityManagement;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out _, out _)) return false;

        if (!(uint.TryParse(arguments.At(0), out var toyid) &&
              NetworkUtils.SpawnedNetIds.TryGetValue(toyid, out var toyNetID)))
        {
            response = "Could not find toy";
            return false;
        }

        if (!(float.TryParse(arguments.At(1), out var x) &&
              float.TryParse(arguments.At(2), out var y) &&
              float.TryParse(arguments.At(3), out var z)))
        {
            response = "Could not parse rotation";
            return false;
        }
        
        toyNetID.gameObject.transform.SetLocalPositionAndRotation(toyNetID.gameObject.transform.localPosition, Quaternion.Euler(new Vector3(x,y,z)));
        
        response = "THe thingy hgas been rotated";
        return true;
    }
}