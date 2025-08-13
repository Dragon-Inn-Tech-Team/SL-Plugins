using System;
using CommandSystem;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using Mirror;
using RedRightHand;
using RedRightHand.Commands;
using UnityEngine;
using Utils.Networking;
using Logger = LabApi.Features.Console.Logger;
using Object = UnityEngine.Object;

namespace Choas.Features.PlayerCommands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GasterVanish : ICustomCommand
{
    public string Command => "gastervanish";

    public string[] Aliases => null;

    public string Description => "Make a player vanish";

    public string[] Usage => ["%player%"];

    public PlayerPermissions? Permission => PlayerPermissions.Effects;

    public string PermissionString => string.Empty;

    public bool RequirePlayerSender => false;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out var plrs, out _)) return false;
        
        var reader = new NVorbis.VorbisReader(ChoasPlugin.Config.AudioClipFolderPath + "GasterVanish.ogg");
        float[] buffer1 = new float[reader.TotalSamples];
        reader.ReadSamples(buffer1, 0, buffer1.Length);
        SpeakerToy.Play(17, buffer1, false);
        foreach (var player in plrs)
        {
            var s = SpeakerToy.Create(player.Position);
            s.ControllerId = 17;
            s.GameObject.AddComponent<selfDestruct>();
            player.EnableEffect<Fade>(1,5f);
            if (player.TryGetEffect(out Fade fade))
            {
                var fadeFader = fade.gameObject.AddComponent<fadeOverTime>();
                fadeFader.fadeEffect = fade;
                Object.Destroy(fadeFader, 5f);
            }
        }
        
        response = $"Made {plrs.Count} player{(plrs.Count == 1 ? "" : "s")} vanish.";
        return true;
    }

    private class selfDestruct : MonoBehaviour // Theres probably a better way to do this stuff lol
    {
        private void Start()
        {
            MEC.Timing.CallDelayed(5f, () => NetworkServer.Destroy(gameObject));
        }
    }

    private class fadeOverTime : MonoBehaviour
    {
        private float updateInterval = 0.12f, timeSinceLastUpdate = 0f, totalTime = 0f, fadeDuration = 0.8f;
        public Fade fadeEffect;
        private void Update()
        {
            if (timeSinceLastUpdate < updateInterval || totalTime > fadeDuration + 2 * updateInterval)
            {
                timeSinceLastUpdate += Time.deltaTime;
                totalTime += Time.deltaTime;
                return;
            }
            
            fadeEffect.Intensity = (byte)Mathf.Lerp(1, 255, totalTime / fadeDuration);
            
            timeSinceLastUpdate = 0;
            totalTime += Time.deltaTime;
        }
    }
}