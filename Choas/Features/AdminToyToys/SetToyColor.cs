using System;
using AdminToys;
using CommandSystem;
using LabApi.Features.Wrappers;
using Mirror;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;
using LightSourceToy = AdminToys.LightSourceToy;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace Choas.Features.AdminToyToys;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetToyColor : ICustomCommand
{
    public string Command => "toycolor";

    public string[] Aliases => null;

    public string Description => "Changes an admin toy's color.";

    public string[] Usage => ["net id", "hex code"];

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

        if (!(toyNetID.gameObject.TryGetComponent(typeof(AdminToyBase), out var comp) && comp is AdminToyBase adminToy))
        {
            response = "Could not find toy object";
            return false;
        }

        if (!ColorUtility.TryParseHtmlString(arguments.At(1), out var color))
        {
            response = "Could not parse color";
            return false;
        }
        
        if (adminToy is PrimitiveObjectToy primitiveToy)
        {
            primitiveToy.NetworkMaterialColor = color;
        } 
        else if (adminToy is LightSourceToy lightSourceToy)
        {
            lightSourceToy.NetworkLightColor = color;
        }
        else
        {
            response = "Could not color the toy";
            return false;
        }
        
        response = "THe thingy hgas been recolored";
        return true;
    }
}