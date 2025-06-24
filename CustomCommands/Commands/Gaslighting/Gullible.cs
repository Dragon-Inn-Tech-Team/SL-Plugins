using AdminToys;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using LabApi.Features.Wrappers;
using Mirror;
using RedRightHand;
using RedRightHand.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TextToy = AdminToys.TextToy;

namespace CustomCommands.Commands.Gaslighting
{
    public class RoofFinder : NetworkBehaviour
    {
        private LayerMask mask = LayerMask.GetMask("OnlyWorldCollision", "Skybox", "Default");
        private DateTime lastUpdate = DateTime.Now;

        void Update()
        {
            if (DateTime.Now.Subtract(lastUpdate).Milliseconds > 250)
                lastUpdate = DateTime.Now;
            else return;
            RaycastHit hit;
            float dist = 10f;
            if (Physics.Raycast(this.transform.parent.position, Vector3.up, out hit, 10, mask))
            {
                dist = hit.distance;
            }
            this.transform.SetPositionAndRotation(this.transform.parent.position + (dist - 0.1f) * Vector3.up, this.transform.rotation);
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Gullible : ICustomCommand
    {
        public string Command => "gullible";

        public string[] Aliases => null;

        public string Description => "Spawns the word \'gullible\' on the ceiling above a player, which follows then and is invisible to them.";

        public string[] Usage => ["%player%"];

        public PlayerPermissions? Permission => PlayerPermissions.Effects;

        public string PermissionString => string.Empty;

        public bool RequirePlayerSender => false;

        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CanRun(this, arguments, out response, out var plrs, out _)) return false;

            var plr = plrs.First();

            var gullible = LabApi.Features.Wrappers.TextToy.Create(Vector3.up, Quaternion.LookRotation(Vector3.up) * Quaternion.Euler(Vector3.forward * 180), plr.GameObject.transform);

            gullible.TextFormat = "<size=2>gullible";

            var performance_guzzler = gullible.GameObject.AddComponent<RoofFinder>();
            
            plr.Connection.Send(new ObjectHideMessage { netId = gullible.Base.netId });

            foreach (var scp in LabApi.Features.Wrappers.Player.List.Where(x => x.IsSCP()))
            {
                scp.Connection.Send(new ObjectHideMessage { netId = gullible.Base.netId });
            }

            response = $"Gullible added. You can remove it with \'DESTROYTOY {gullible.Base.netId}\'";
            if (plrs.Count > 1) {
                response += "\n(I'm lazy so you can only do one person at a time. Only the first person's ID was used)";
            }
            return true;
        }
    }
}
