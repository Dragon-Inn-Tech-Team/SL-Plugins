using AdminToys;
using MapGeneration;
using Mirror;
using PluginAPI.Core;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Features.Map.SurfaceLightFix
{
	public class SurfaceLightObject : MonoBehaviour
	{
		private RoomLightController controller;
		private bool ready = false;
		private LightSourceToy surfaceLight;
		private float fadeTimer = 0f;
		private const float fadeDuration = 2f;
		private const float lightIntensity = 50f;

		private void Start()
		{
			Log.Info("Surface light starting");
			controller = RoomLightController.Instances.Find(x => x.Room.Name == RoomName.Outside);
			if (controller == default)
			{
				Log.Warning("Surface light controller not found, will try again in 10s");
				MEC.Timing.CallDelayed(10f, () =>
				{
					Start();
				});
				return;
			}

			var lightGO = GameObject.Instantiate(NetworkClient.prefabs.First(r => r.Value.name == "LightSourceToy").Value);
			lightGO.transform.position = new Vector3(135, 1024, -43);
			NetworkServer.Spawn(lightGO);
			surfaceLight = lightGO.GetComponent<LightSourceToy>();

			surfaceLight.NetworkLightIntensity = lightIntensity;
			surfaceLight.NetworkLightRange = 250;
			surfaceLight.NetworkLightColor = Color.white;
			surfaceLight.NetworkShadowType = LightShadows.None;
			ready = true;
		}

		private float previousTargetIntensity = lightIntensity;
		private void Update()
		{
			if (NetworkServer.active && ready)
			{
				float targetIntensity = controller.LightsEnabled ? lightIntensity : 0f;

				fadeTimer += Time.deltaTime;

				if (fadeTimer <= fadeDuration)
					surfaceLight.NetworkLightIntensity = Mathf.Lerp(surfaceLight.NetworkLightIntensity, targetIntensity, fadeTimer / fadeDuration);

				if (previousTargetIntensity != targetIntensity)
					fadeTimer = 0;

				previousTargetIntensity = targetIntensity;

				surfaceLight.NetworkLightColor = AlphaWarheadController.InProgress ? Color.red : Color.white;
			}
		}
	}
}
