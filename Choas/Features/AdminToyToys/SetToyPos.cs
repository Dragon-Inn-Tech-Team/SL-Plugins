using System;
using System.Linq;
using System.Numerics;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using Utils.Networking;
using Vector3 = UnityEngine.Vector3;

namespace Choas.Features.AdminToyToys;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetToyPos : ICustomCommand
{
    public string Command => "toypos";

    public string[] Aliases => null;

    public string Description => "Moves an admin toy to a specified (local) position.";

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
            response = "Could not parse coordinates";
            return false;
        }
        
        toyNetID.gameObject.transform.SetLocalPositionAndRotation(new Vector3(x,y,z), toyNetID.gameObject.transform.rotation);
        
            
        response = "THe thingy hgas been moved";
        return true;
    }
}