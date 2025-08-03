using System;
using System.Linq;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using Utils.Networking;

namespace Choas.Features.AdminToyToys.ToyLookAt;

[CommandHandler(typeof(MakeToyLookAtParent))]
public class ToyLookAtToy : ICustomCommand
{
    public string Command => "toy";

    public string[] Aliases => null;

    public string Description => "Makes a toy look at another toy.";

    public string[] Usage => ["child id", "parent id"];

    public PlayerPermissions? Permission => PlayerPermissions.FacilityManagement;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out _, out _)) return false;
        
        if (!(uint.TryParse(arguments.At(0), out var ctoyid) &&
              NetworkUtils.SpawnedNetIds.TryGetValue(ctoyid, out var childToyNetID)))
        {
            response = "Could not find child toy.";
            return false;
        }
        
        if (!(uint.TryParse(arguments.At(1), out var ptoyid) &&
              NetworkUtils.SpawnedNetIds.TryGetValue(ptoyid, out var parentToyNetID)))
        {
            response = "Could not find parent toy.";
            return false;
        }

        childToyNetID.gameObject.transform.LookAt(parentToyNetID.transform.position);

        response = "THe thingy hgas been rotated";
        return true;
    }
}