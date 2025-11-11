using CustomPlayerEffects;
using HarmonyLib;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using LabApi.Features.Wrappers;
using System;
using UnityEngine;
using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;

namespace CustomCommands.Features.SCP3114Enable
{
	public class SCP3114Overhaul : CustomFeature
	{
		public SCP3114Overhaul(bool configSetting) : base(configSetting)
		{
		}

		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
		{
			if (ev.Player.Role == RoleTypeId.Scp3114)
			{
				Timing.CallDelayed(0.2f, () =>
				{
					var role = Disguise3114(ev.Player);

					role.SpawnpointHandler.TryGetSpawnpoint(out Vector3 pos, out float rot);

					ev.Player.Position = pos;
					var scpid = (ev.Player.RoleBase as Scp3114Role);
					scpid.FpcModule.MouseLook.CurrentHorizontal = rot;
				});
			}
		}

		//public override void OnPlayerHurt(PlayerHurtEventArgs ev)
		//{
		//	if (ev.DamageHandler is Scp3114DamageHandler sDH && sDH.Subtype == Scp3114DamageHandler.HandlerType.Slap)
		//	{
		//		ev.Player.EnableEffect<Hemorrhage>(duration: 10);
		//	}
		//}

		public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
		{
			if (ev.Player.Role == RoleTypeId.Scp3114)
			{
				if (ev.Pickup.Category == ItemCategory.SCPItem || ev.Pickup.Category == ItemCategory.SpecialWeapon)
				{
					ev.Player.SendHint($"You cannot pick up this item", 3);
					ev.IsAllowed = false;
				}
			}
		}

		public static HumanRole Disguise3114(Player plr)
		{
			#region Spawns Ragdoll

			RoomUtils.TryFindRoom(RoomName.EzEvacShelter, FacilityZone.Entrance, RoomShape.Endroom, out RoomIdentifier roomIdentifier);
			Transform transform = roomIdentifier.transform;

			RoleTypeId role = new System.Random().Next(0, 2) == 1 ? RoleTypeId.ClassD : RoleTypeId.Scientist;

			PlayerRoleLoader.TryGetRoleTemplate<HumanRole>(role, out HumanRole humanRole);

			BasicRagdoll basicRagdoll = UnityEngine.Object.Instantiate<BasicRagdoll>(humanRole.Ragdoll);
			basicRagdoll.NetworkInfo = new RagdollData(null, new Scp3114DamageHandler(basicRagdoll, true), role, new RelativePositioning.RelativePosition(transform.position), transform.rotation, plr.Nickname, NetworkTime.time);
			NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);

			#endregion

			var scpid = (plr.ReferenceHub.roleManager.CurrentRole as Scp3114Role);
			scpid.CurIdentity.Ragdoll = basicRagdoll;
			scpid.Disguised = true;

			return humanRole;
		}
	}

	///This is all disabled due to 14.2 and the changes NW have made to the changes regarding stranglation

	//[HarmonyPatch(typeof(Scp3114Reveal))]
	//[HarmonyPatch("ServerProcessCmd")]
	//public class Scp3114RevealPatchClass
	//{
	//	public static bool canStrangle = false;
	//	[HarmonyPrefix]
	//	public static bool prefix(Scp3114Reveal __instance)
	//	{
	//		if (Round.Duration < TimeSpan.FromSeconds(60))
	//		{
	//			Player.Get(__instance.Owner).SendHint("You cannot yet undisguise");
	//			return false;
	//		}
	//		else
	//		{
	//			canStrangle = true;
	//			return true;
	//		}
	//	}
	//}

	//[HarmonyPatch(typeof(Scp3114Strangle))]
	//[HarmonyPatch("ValidateTarget")]
	//public class Scp3114StranglePatchClass
	//{
	//	[HarmonyPostfix]
	//	public static void postfix(Scp3114Strangle __instance, ref bool __result, ReferenceHub player)
	//	{
	//		if (!Scp3114RevealPatchClass.canStrangle)
	//		{
	//			Player.Get(__instance.Owner).SendHint("You must disguise yourself to use Strangulation again!");
	//			__result = false;
	//		}

	//		Scp3114RevealPatchClass.canStrangle = false;

	//		if (player.playerStats.GetModule<HealthStat>().CurValue > 60)
	//		{
	//			Player.Get(__instance.Owner).SendHint("They are still too strong to strangle");
	//			__result = false;
	//		}
	//	}
	//}
}
