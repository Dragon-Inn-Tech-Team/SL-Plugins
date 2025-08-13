using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choas.Features.OldMechanics;
using Choas.Features.PinkCandy;
using Choas.SSSettings;
using CustomCommands.Core;
using LabApi.Events.CustomHandlers;
using RedRightHand.CustomPlugin;
using UserSettings.ServerSpecific;

namespace Choas
{
    public class ChoasPlugin : CustomPluginCore<Config>
    {
        public override string Name => "Choas";

        public override string Description => "brings the choas to SL";

        public override Version Version => new(1, 0, 0);

        public override string ConfigFileName => "ChoasPlugin.yml";
        
        public override void LoadConfigs()
        {
            Config = LoadPluginConfigs<Config>();
        }
        
		public CustomFeature[] features;

        public override void Enable()
        {
            if (Config.EnableCustomKeybinds)
            {
                CustomHandlersManager.RegisterEventsHandler(Events);

                if (ServerSpecificSettingsSync.DefinedSettings == null)
                    ServerSpecificSettingsSync.DefinedSettings = [];

                ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings
                    .Concat(CustomSettingsManager.GetAllSettings()).ToArray();
                ServerSpecificSettingsSync.SendToAll();
            }

            features =
            [
                new OldMechanicsEvents(Config.EnableOldMechanics),
                new CandyGrabEvents(Config.EnablePinkCandyEvents, Config.PinkCandyChance),
            ];
        }

        public override void Disable() { }
    }
}
