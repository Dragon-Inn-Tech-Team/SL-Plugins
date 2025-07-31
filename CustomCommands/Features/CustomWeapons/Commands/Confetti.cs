using CommandSystem;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.CustomWeapons.Commands
{
    [CommandHandler(typeof(CustomWeaponParent))]
    public class Confetti : ICustomCommand
    {
        public string Command => "confetti";

        public string[] Aliases { get; } = { "cl" };

        public string Description => "Launches confetti when you shoot your gun";

        public string[] Usage { get; } = { };

        public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
        public string PermissionString => string.Empty;

        public bool RequirePlayerSender => true;

        public bool SanitizeResponse => false;
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
                return false;

            var plr = Player.Get(pSender.ReferenceHub);

            if (!CustomWeaponsManager.AvailableWeapons.TryGetValue(CustomWeaponsManager.CustomWeaponType.Confetti, out var customWeaponBase))
            {
                response = $"Unable to find weapon with type {CustomWeaponsManager.CustomWeaponType.Confetti}";
                return false;
            }

            bool enabled = customWeaponBase.ToggleWeapon(plr);
            
            response = $"Confetti launcher {(enabled ? "enabled" : "disabled")}.";
            return true;
        }
    }
}
