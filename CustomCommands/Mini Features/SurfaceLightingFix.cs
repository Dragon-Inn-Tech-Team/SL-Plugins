using CustomCommands.Core;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCommands.Features.SurfaceLightingFix
{
	public class SurfaceLightingFix : CustomFeatureBase
	{
		List<LightSourceToy> lightSourceToys = new List<LightSourceToy>();

		public SurfaceLightingFix(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerWaitingForPlayers()
		{
			lightSourceToys.Clear();

			var lightToy = LightSourceToy.Create(new Vector3(135, 324, -43));

			lightToy.Intensity = 50f;
			lightToy.Range = 250;
			lightToy.Color = Color.white;
			lightToy.ShadowType = LightShadows.None;

			lightSourceToys.Add(lightToy);
		}

		public override void OnWarheadStarted(WarheadStartedEventArgs ev)
		{
			foreach (var light in lightSourceToys)
				light.Color = Color.red;
		}

		public override void OnWarheadStopped(WarheadStoppedEventArgs ev)
		{
			foreach (var light in lightSourceToys)
				light.Color = Color.white;
		}
	}
}
