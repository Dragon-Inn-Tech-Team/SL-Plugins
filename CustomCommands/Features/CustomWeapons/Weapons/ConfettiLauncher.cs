using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomCommands.Features.CustomWeapons.Weapons;

public class ConfettiLauncher : CustomWeaponBase
{
    public override CustomWeaponsManager.CustomWeaponType WeaponType => CustomWeaponsManager.CustomWeaponType.Confetti;
    public override string Name =>  "Confetti Launcher";
    public static readonly List<string> ConfettiColors = ["a864fd", "29cdff", "78ff44", "ff718d", "fdff6a"];
    
    public override void ShootWeapon(Player player)
    {
        var confetto = TextToy.Create(player.Camera.position, player.Camera.rotation);
        confetto.TextFormat = $"<color=#{ConfettiColors.RandomItem()}>■";
        confetto.Scale = new(0.05f, 0.2f, 0.05f); // Too lazy to test if its x or z that needs to be squished since the other is flat already

        var rb = confetto.GameObject.AddComponent<Rigidbody>();
        
        rb.detectCollisions = false;
        
        // Lots of vibe based numbers from testing
        rb.mass = 0.1f;
        rb.linearDamping = 5f; 
        rb.angularDamping = 0.01f;
        rb.angularVelocity = Random.onUnitSphere*3;
        rb.AddForce(player.Camera.forward*100 + Random.onUnitSphere*20);
        
        _ = confetto.GameObject.AddComponent<NetworkRigidbodyUnreliable>(); 
        confetto.GameObject.AddComponent<SelfDestructProjectile>().DelayedDestroy(5f);
    }
}