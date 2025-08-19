using DynamicTags.Systems;
using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using RedRightHand.CustomPlugin;
using System;

namespace DynamicTags
{
	public class DynamicTagsPlugin : CustomPluginCore<Config>
	{
		public static Config Config;

		public override string ConfigFileName => "DynamicTagsConfig.yml";

		public override string Name => "Dynamic Tags & Tracker";

		public override string Description => "Simple plugin to handle dynamic tags and player tracking via external APIs";

		public override Version Version => new(1,1,0);

		public Reporting Reporting { get; set; }
		public StaffTracker StaffTracker { get; set; }
		public AltChecker AltTracking { get; set; }

		public override void LoadConfigs()
		{
			base.LoadConfigs();
			Config = LoadPluginConfigs<Config>();
		}

		public override void Disable()
		{
			//Registers the events used in the DynamicTags class
			if (Config.TagsEnabled)
			{
				Events = new Systems.DynamicTags();
				CustomHandlersManager.UnregisterEventsHandler(Events);
			}
			if (Config.TrackerEnabled)
			{
				StaffTracker = new StaffTracker();
				CustomHandlersManager.UnregisterEventsHandler(StaffTracker);
			}

			Reporting = new Reporting();
			CustomHandlersManager.UnregisterEventsHandler(Reporting);
		}

		public override void Enable()
		{
			Logger.Info($"Plugin is loading...");

			Harmony harmony = new Harmony("DT-Patching-Phegg");
			harmony.PatchAll();

			//Registers the events used in the DynamicTags class
			if (Config.TagsEnabled)
			{
				Events = new Systems.DynamicTags();
				CustomHandlersManager.RegisterEventsHandler(Events);
			}
			if (Config.TrackerEnabled)
			{
				StaffTracker = new StaffTracker();
				CustomHandlersManager.RegisterEventsHandler(StaffTracker);
			}

			Reporting = new Reporting();
			CustomHandlersManager.RegisterEventsHandler(Reporting);

			Logger.Info($"Plugin is loaded. API Endpoint is: {Config.ApiUrl}");
		}
	}
}
