using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedRightHand.Components
{
    /// <summary>
    /// Add this to the player game object as a component
    /// </summary>
    public class KillFeed : MonoBehaviour
    {
        public int numLines = 5;
        /// <summary>
        /// Set this if you want team colors to show in the kill feed.
        /// </summary>
        public Team PlrTeam = Team.OtherAlive;
        private NetworkConnectionToClient playerConnection;
        private string plrName;
        private Queue<string> history = [];
        private void addToHistory(string text)
        {
            history.Enqueue(text);
            if (history.Count > numLines)
                history.Dequeue();
        }
        private string history_toString()
        {
            string bc = "";
            foreach (var s in history)
                bc += s + "\n";
            return bc;
        }

        public KillFeed(Player plr)
        {
            playerConnection = plr.ReferenceHub.connectionToClient;
            plrName = plr.Nickname;
        }

        public static string GetTeamColor(Team team)
        {
            switch (team)
            {
                case Team.SCPs:
                    return "red";
                case Team.Scientists:
                case Team.FoundationForces:
                    return "blue";
                case Team.ClassD:
                case Team.ChaosInsurgency:
                    return "green";
                default:
                    return "white";
            }
        }

        /// <summary>
        /// Hook this into a multicast delegate function for the PlayerDeath event for the kill feed to function.
        /// </summary>
        /// <param name="args"></param>
        public void UpdateFeed(PlayerDeathEventArgs args)
        {
            string victim = $"<color={GetTeamColor(args.Player.Team)}>" + args.Player.Nickname + "</color>";
            string attacker = victim;
            bool bold = args.Player.Team == PlrTeam || args.Player.Nickname.Equals(plrName);
            if (args.Attacker != null)
            {
                attacker = $"<color={GetTeamColor(args.Attacker.Team)}>" + args.Attacker.Nickname + "</color>";
                bold = bold || args.Attacker.Team == PlrTeam || args.Attacker.Nickname.Equals(plrName);
            }
            string line = $"<size=-14><align=left><pos=-8em>{(bold ? "<b>": "")}" +
                    $"{attacker} killed {victim}" +
                    $"{(bold ? "</b>" : "")}</align></pos></size>";

            addToHistory(line);

            Broadcast.Singleton.TargetClearElements(playerConnection);
            Broadcast.Singleton.TargetAddElement(playerConnection, history_toString(), 15, Broadcast.BroadcastFlags.Normal);
        }
    }
}
