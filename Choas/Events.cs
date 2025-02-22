﻿using Choas.Components;
using LabApi.Events;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Choas
{
    public class Events : CustomEventsHandler
    {
        public override void OnPlayerSpawned(PlayerSpawnedEventArgs args) //Chance for two 939s to spawn and 173 speed tied to HP, like old times
        {
            if (args.Player.Team != PlayerRoles.Team.SCPs) return;
            if (args.Player.Role != PlayerRoles.RoleTypeId.Scp0492 && ReferenceHub.AllHubs.Where(x => x.roleManager.CurrentRole.RoleTypeId == PlayerRoles.RoleTypeId.Scp939).Count() == 1)
            {
                System.Random rng = new System.Random();
                if (rng.Next(0, 5) == 0) //Chance is lower than this because it also relies on a 939 already existing
                {
                    args.Player.SetRole(PlayerRoles.RoleTypeId.Scp939, PlayerRoles.RoleChangeReason.RoundStart);
                } 
            } else if (args.Player.Role == PlayerRoles.RoleTypeId.Scp173)
            {
                var hts = args.Player.GameObject.AddComponent<HealthToSpeed>(); //Using spawned event instead of spawning cus of this, might break otherwise
                hts.plr = args.Player;
            }
        }

        public override void OnPlayerDeath(PlayerDeathEventArgs args) { 
            if (args.Player.Role == PlayerRoles.RoleTypeId.Scp173 && args.Player.GameObject.TryGetComponent<HealthToSpeed>(out var hts))
            {
                UnityEngine.Object.Destroy(hts);
            }
        }
    }
}
