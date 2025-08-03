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
public class Load : ICustomCommand
{
    public string Command => "play";

    public string[] Aliases => null;

    public string Description => "Makes a speaker toy play a sound. Run blank to see available sounds.";

    public string[] Usage => ["net id", "file name", "queue? (y/n)", "loop? (y/n)"];

    public PlayerPermissions? Permission => PlayerPermissions.FacilityManagement;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count == 0)
        {
            if (ChoasPlugin.Config.AudioClipFolderPath.Equals(""))
            {
                response = "Sound file folder has not been specified.";
                return false;
            }

            var soundFiles = Directory.GetFiles(ChoasPlugin.Config.AudioClipFolderPath, "*.ogg");
            response = "Sound files available: ";
            foreach (var file in soundFiles)
            {
                response += file.Replace(".ogg","").Replace(ChoasPlugin.Config.AudioClipFolderPath, "") + ", ";
            }
            return true;
        }

        if (arguments.Count == 2)
        {
            arguments = arguments.Append("n").Append("n").ToArray().Segment(0); // For all my lazy homies out there (me)
        }
        
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

        var filePath = ChoasPlugin.Config.AudioClipFolderPath + arguments.At(1) + ".ogg";
        
        if (ChoasPlugin.Config.AudioClipFolderPath.Equals("") || !File.Exists(filePath))
        {
            response = "Could not find specified sound file.";
            return false;
        }
        
        NVorbis.VorbisReader test = new(filePath);

        var bufferLength = test.TotalSamples;
        float[] buffer = new float[bufferLength];
        test.ReadSamples(buffer, 0, buffer.Length);
        speakerToy.Play(buffer, arguments.At(2).ToLower().Equals("y"), arguments.At(3).ToLower().Equals("y"));

        response = "THe thingy hgas been sounded";
        return true;
    }
}