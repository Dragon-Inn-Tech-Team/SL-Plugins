using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserSettings.ServerSpecific;

namespace RedRightHand.Core.CustomSettings
{
	public static class CustomSettingsManager
	{
		public static ServerSpecificSettingBase[] ActivateAllSettings(CustomSettingsBase[] settings)
		{
			List<ServerSpecificSettingBase> ssSettingBases = new List<ServerSpecificSettingBase>();

			foreach (var customSettings in settings)
			{
				customSettings.Activate();
				ssSettingBases.AddRange(customSettings.SettingBases);
			}

			return ssSettingBases.ToArray();
		}

		public static ServerSpecificSettingBase[] DeactivateAllSettings(CustomSettingsBase[] settings)
		{
			List<ServerSpecificSettingBase> ssSettingBases = new List<ServerSpecificSettingBase>();

			foreach (var customSettings in settings)
			{
				customSettings.Deactivate();
				ssSettingBases.AddRange(customSettings.SettingBases);
			}

			return ssSettingBases.ToArray();
		}
	}
}
