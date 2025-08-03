using System.Linq;
using Choas.Components;
using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace Choas.Features.OldMechanics
{
    public class OldMechanicsEvents : CustomFeature
    {
        public OldMechanicsEvents(bool configSetting) : base(configSetting)
        {
            if (!isEnabled) return;
            Logger.Info("What is this, 2019?");
        }

        public override void OnPlayerSpawned(PlayerSpawnedEventArgs args) //Chance for two 939s to spawn and 173 speed tied to HP, like old times
        {
            if (args.Player.Team != PlayerRoles.Team.SCPs) return;
            if (args.Player.Role != PlayerRoles.RoleTypeId.Scp0492 && ReferenceHub.AllHubs.Where(x => x.roleManager.CurrentRole.RoleTypeId == PlayerRoles.RoleTypeId.Scp939).Count() == 1)
            {
                if (Random.Range(0, 5) == 0) //Chance is lower than this because it also relies on a 939 already existing
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
