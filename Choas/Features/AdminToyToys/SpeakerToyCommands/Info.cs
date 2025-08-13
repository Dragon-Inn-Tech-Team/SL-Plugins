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
public class Info : ICustomCommand
{
    public string Command => "info";

    public string[] Aliases => null;

    public string Description => "Gives info about the speaker toy and the sound queue.";

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
        
        response = "Info:\n";
        response += $"Current position - {speakerToy.CurrentPosition}\n";
        response += $"Sound duration - {speakerToy.CurrentDuration}\n";
        response += $"Remaining duration - {speakerToy.CurrentRemainingDuration}\n";
        response += $"Is playing? - {(speakerToy.IsPlaying ? "yes" : "no")}\n";
        response += $"Is paused? - {(speakerToy.IsPaused ? "yes" : "no")}\n";
        response += $"Is looping? - {(speakerToy.IsLooping ? "yes" : "no")}\n";
        response += $"Queued clips - {speakerToy.QueuedClipsCount}\n";
        response += $"Queue duration - {speakerToy.QueuedClipsDuration}\n";
        response += "(technical stuff:)\n";
        response += $"Is spatial? - {(speakerToy.IsSpatial ? "yes" : "no")}\n";
        response += $"Controller ID - {speakerToy.ControllerId}\n";
        response += $"Max distance - {speakerToy.MaxDistance}\n";
        response += $"Min distance - {speakerToy.MinDistance}\n";
        
        return true;
    }
}