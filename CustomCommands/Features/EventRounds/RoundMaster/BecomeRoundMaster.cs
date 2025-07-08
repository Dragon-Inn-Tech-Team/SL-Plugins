using System;
using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;

namespace CustomCommands.Features.EventRounds.RoundMaster;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class BecomeRoundMaster : ICustomCommand
{
    public string Command => "roundmaster";

    public string[] Aliases => null;

    public string Description => "Become the Round Master";

    public string[] Usage => null;

    public PlayerPermissions? Permission => null;

    public string PermissionString => "cuscom.roundmaster";

    public bool RequirePlayerSender => true;

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CanRun(this, arguments, out response, out _, out var plr)) return false;
        if (!CustomCommandsPlugin.Config.EnableRoundMaster)
        {
            response = "Round Master is not enabled on this server.";
            return false;
        }

        var me = plr.ReferenceHub;
            
        UserGroup group = (UserGroup) null;
        group = ServerStatic.PermissionsHandler.GetGroup("roundmaster");
        if (group == null)
        {
            response = "The Round Master group is not set up on this server.";
            return false;
        }
        
        ServerLogs.AddLog(ServerLogs.Modules.Permissions, $"{sender.LogName} set local group of player {me.LoggedNameFromRefHub()} to roundmaster.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
        me.serverRoles.SetGroup(group, true, true);

        response = "Round Master permissions granted!";
        return true;
    }
}