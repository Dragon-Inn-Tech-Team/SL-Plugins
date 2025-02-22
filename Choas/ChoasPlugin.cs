using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choas.SSSettings;
using RedRightHand.CustomPlugin;
using UserSettings.ServerSpecific;

namespace Choas
{
    public class ChoasPlugin : CustomPluginCore
    {
        public override string Name => "Choas";

        public override string Description => "brings the choas to SL";

        public override Version Version => new(1, 0, 0);

        public override void Enable()
        {
            CustomHandlersManager.RegisterEventsHandler(Events);
        
            if (ServerSpecificSettingsSync.DefinedSettings == null)
                ServerSpecificSettingsSync.DefinedSettings = [];

            ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Concat(CustomSettingsManager.GetAllSettings()).ToArray();
            ServerSpecificSettingsSync.SendToAll();
        }

        public override void Disable() { }
    }
}
