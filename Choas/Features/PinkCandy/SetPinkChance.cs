using System;
using System.Linq;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;

namespace Choas.Features.PinkCandy;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetPinkChance : ICustomCommand
{
    public string Command => "pinkchance";

    public string[] Aliases => null;

    public string Description => "Sets the chance of getting pink candy for one round.";

    public string[] Usage => ["% chance"];

    public PlayerPermissions? Permission => PlayerPermissions.Effects;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out _, out _)) return false;

        if (float.TryParse(arguments.FirstOrDefault(), out var pinkChance))
        {
            CandyGrabEvents.PinkChance = pinkChance;
        }
        else
        {
            response = "Could not parse the given number!";
            return false;
        }
        
        response = $"There is now a {Mathf.Clamp(pinkChance, 0, 100)}% chance of getting pink candy.";
        return true;
    }
}