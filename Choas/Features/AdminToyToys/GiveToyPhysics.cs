using System;
using CommandSystem;
using Mirror;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;

namespace Choas.Features.AdminToyToys;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GiveToyPhysics : ICustomCommand
{
    public string Command => "toyphys";

    public string[] Aliases => null;

    public string Description => "Gives an admin toy physics.";

    public string[] Usage => ["net id"];

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

        _ = toyNetID.gameObject.AddComponent<Rigidbody>();
        _ = toyNetID.gameObject.AddComponent<NetworkRigidbodyUnreliable>();
        
        response = "THe thingy hgas been physic'd";
        return true;
    }
}