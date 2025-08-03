using System;
using System.IO;
using System.Linq;
using AdminToys;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using Utils.Networking;
using SpeakerToy = LabApi.Features.Wrappers.SpeakerToy;

namespace Choas.Features.AdminToyToys.SpeakerToyCommands;

[CommandHandler(typeof(ControlSpeakerToy))]
public class Loop : ICustomCommand
{
    public string Command => "loop";

    public string[] Aliases => null;

    public string Description => "Sets a speaker toy to loop.";

    public string[] Usage => ["net id"];

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

        if (!toyNetID.gameObject.TryGetComponent(typeof(AdminToyBase), out var comp) || comp is not AdminToyBase adminToy || adminToy is not AdminToys.SpeakerToy speakerToyBase || !SpeakerToy.TryGet(speakerToyBase, out var speakerToy))
        {
            response = "Could not find speaker toy object";
            return false;
        }
        
        speakerToy.IsLooping = !speakerToy.IsLooping;
        
        response = $"THe thingy hgas been set to {(speakerToy.IsLooping ? "loop" : "play once")}";
        return true;
    }
}