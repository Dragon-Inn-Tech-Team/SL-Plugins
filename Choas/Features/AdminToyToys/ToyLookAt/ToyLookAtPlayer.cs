using System;
using System.Linq;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;

namespace Choas.Features.AdminToyToys.ToyLookAt;

[CommandHandler(typeof(MakeToyLookAtParent))]
public class ToyLookAtPlayer : ICustomCommand
{
    public string Command => "player";

    public string[] Aliases => null;

    public string Description => "Makes a toy look at a player.";

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
            response = "Could not find toy";
            return false;
        }

        toyNetID.gameObject.transform.LookAt(plr.GameObject.transform);

        response = "THe thingy hgas been rotated";
        return true;
    }
}