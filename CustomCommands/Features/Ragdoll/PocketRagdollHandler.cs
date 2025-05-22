using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core;
using System;
using System.Reflection;
using UnityEngine;

namespace CustomCommands.Features.Ragdoll
{
	public class PocketRagdollHandler
	{
		public static void RagdollManager_OnRagdollSpawned(BasicRagdoll obj)
		{
			if (!obj.TryGetComponent<FakeRagdoll>(out _) && obj.Info.Handler is UniversalDamageHandler uDH && uDH.TranslationId == DeathTranslations.PocketDecay.Id)
			{
				try
				{
					FacilityZone zone = (FacilityZone)UnityEngine.Random.Range(1, 4);

					var bFlags = BindingFlags.NonPublic | BindingFlags.Static;
					DoorVariant[] doors = (DoorVariant[])typeof(Scp106PocketExitFinder).GetMethod("GetWhitelistedDoorsForZone", bFlags).Invoke(null, new object[] { zone });
					DoorVariant door = (DoorVariant)typeof(Scp106PocketExitFinder).GetMethod("GetRandomDoor", bFlags).Invoke(null, new object[] { doors });

					PlayerRoleLoader.TryGetRoleTemplate(obj.Info.RoleType, out HumanRole ragdollRole);
					BasicRagdoll basicRagdoll = UnityEngine.Object.Instantiate<BasicRagdoll>(ragdollRole.Ragdoll);

					var pos = door.transform.position;

					if (UnityEngine.Random.Range(1, 2) % 2 == 0)
						pos += door.transform.forward * 0.25f;
					else
						pos -= door.transform.forward * 0.25f;
					pos += door.transform.up * 0.25f;

					basicRagdoll.NetworkInfo = new RagdollData(obj.Info.OwnerHub, obj.Info.Handler, obj.Info.RoleType, pos, obj.Info.StartRotation, obj.Info.Nickname, obj.Info.CreationTime);
					basicRagdoll.gameObject.AddComponent<FakeRagdoll>();
					NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
					NetworkServer.Destroy(obj.gameObject);
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}
			}
		}
	}

	public class FakeRagdoll : MonoBehaviour
	{

	}
}
