using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;

namespace CustomCommands.Core
{
	public abstract class CustomFeatureBase : CustomEventsHandler
	{
		public static CustomFeatureBase Instance { get; private set; }
		public bool IsEnabled { get; private set; }

		public CustomFeatureBase(bool configSetting)
		{
			Instance = this;

			if (configSetting)
				OnEnabled();
		}

		public virtual void OnEnabled()
		{
			IsEnabled = true;
			Logger.Info($"LOADING {this.GetType().Name}");

			CustomHandlersManager.RegisterEventsHandler(this);

			Logger.Info(IsEnabled);
		}
		public virtual void OnDisabled()
		{
			Logger.Info($"UNLOADING {this.GetType().Name}");
			IsEnabled = false;
			CustomHandlersManager.UnregisterEventsHandler(this);
		}
	}
}
