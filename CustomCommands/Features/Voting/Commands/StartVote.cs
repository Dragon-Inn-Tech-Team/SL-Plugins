using CommandSystem;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.Voting.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartVote : ICustomCommand
    {
        public string Command => "vote";

        public string[] Aliases => null;

        public string Description => "Triggers a vote for players";

        public string[] Usage { get; } = { "message" };

        public PlayerPermissions? Permission => null;
        public string PermissionString => "cuscom.playervote";

        public bool RequirePlayerSender => true;

        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CanRun(this, arguments, out response, out var players, out var pSender))
                return false;

            if (Features.Voting.Votes.VoteInProgress)
            {
                response = "There is already a vote in progress";
                return false;
            }

            var msg = string.Join(" ", arguments).Replace(Command, string.Empty).Trim();
            Features.Voting.Votes.SetVote(VoteType.AdminVote, msg);

            foreach (var a in Player.List)
            {
                a.SendBroadcast($"<b><color=#fa886b>[VOTE]</color></b> <color=#79b7ed>{msg}</color>\nUse your console (Press ' to open) to vote now!", 15);
                a.SendConsoleMessage("A vote has been started. Run the `.yes` command to vote yes, or `.no` command to vote no");
            }

            MEC.Timing.CallDelayed(3 * 60, () =>
            {
                Features.Voting.Votes.EndVote();

            });
            response = "Vote has been started";
            return true;
        }
    }
}
