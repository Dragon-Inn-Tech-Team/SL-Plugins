using CustomCommands.Core;
using InventorySystem.Items.Usables.Scp330;
using LabApi.Events.Arguments.PlayerEvents;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace Choas.Features.PinkCandy;

public class CandyGrabEvents : CustomFeature
{
    /// <summary>
    /// The % chance, i.e. from 0 to 100
    /// </summary>
    public static float PinkChance = -1f;

    public CandyGrabEvents(bool configSetting, float pinkChance) : base(configSetting)
    {
        if (!isEnabled) return;
        Logger.Info($"Chance of pink candy: {Mathf.Clamp(pinkChance, 0, 100)}%");
        PinkChance = pinkChance;
    }

    public override void OnPlayerInteractingScp330(PlayerInteractingScp330EventArgs ev)
    {
        if (PinkChance > 0 && Random.Range(0f,100f) <= PinkChance)
            ev.CandyType = CandyKindID.Pink;
    }

    public override void OnServerRoundRestarted()
    {
        PinkChance = ChoasPlugin.Config.PinkCandyChance;
    }
}