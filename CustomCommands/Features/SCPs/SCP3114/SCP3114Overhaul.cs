using CustomPlayerEffects;
using HarmonyLib;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using UnityEngine;

namespace CustomCommands.Features.SCPs.SCP3114
{
	public class SCP3114Overhaul
	{
		[PluginEvent]
		public void PlayerSpawn(PlayerSpawnEvent args)
		{
			if (args.Player.Role == RoleTypeId.Scp3114)
			{
				Timing.CallDelayed(0.2f, () =>
				{
					var role = Disguise3114(args.Player);

					role.SpawnpointHandler.TryGetSpawnpoint(out Vector3 pos, out float rot);

					args.Player.Position = pos;
					var scpid = (args.Player.RoleBase as Scp3114Role);
					scpid.FpcModule.MouseLook.CurrentHorizontal = rot;
				});
			}


		}

		[PluginEvent]
		public void PlayerDamaged(PlayerDamageEvent args)
		{
			if (args.DamageHandler is Scp3114DamageHandler sDH && sDH.Subtype == Scp3114DamageHandler.HandlerType.Slap)
			{
				args.Target.EffectsManager.EnableEffect<Hemorrhage>(10);

			}
		}

		[PluginEvent]
		public bool PlayerItemPickupEvent(PlayerSearchPickupEvent ev)
		{
			if (ev.Player.Role == RoleTypeId.Scp3114)
			{
				if (ev.Item.Info.ItemId == ItemType.MicroHID || ev.Item.Info.ItemId == ItemType.SCP268 || ev.Item.Info.ItemId == ItemType.SCP207)
				{
					ev.Player.ReceiveHint($"You cannot pick up this item", 3);
					return false;
				}
			}
			return true;
		}

		public static HumanRole Disguise3114(Player plr)
		{
			#region Spawns Ragdoll

			RoomIdUtils.TryFindRoom(RoomName.EzEvacShelter, FacilityZone.Entrance, RoomShape.Endroom, out RoomIdentifier roomIdentifier);
			Transform transform = roomIdentifier.transform;

			RoleTypeId role = new System.Random().Next(0, 2) == 1 ? RoleTypeId.ClassD : RoleTypeId.Scientist;

			PlayerRoleLoader.TryGetRoleTemplate<HumanRole>(role, out HumanRole humanRole);

			BasicRagdoll basicRagdoll = UnityEngine.Object.Instantiate<BasicRagdoll>(humanRole.Ragdoll);
			basicRagdoll.NetworkInfo = new RagdollData(null, new Scp3114DamageHandler(basicRagdoll, true), role, transform.position, transform.rotation, plr.Nickname, NetworkTime.time);
			NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);

			#endregion

			var scpid = (plr.ReferenceHub.roleManager.CurrentRole as Scp3114Role);
			scpid.CurIdentity.Ragdoll = basicRagdoll;
			scpid.Disguised = true;

			return humanRole;
		}
	}

	[HarmonyPatch(typeof(Scp3114Reveal))]
	[HarmonyPatch("ServerProcessCmd")]
	public class Scp3114RevealPatchClass
	{
		public static bool canStrangle = false;
		[HarmonyPrefix]
		public static bool prefix(Scp3114Reveal __instance)
		{
			if (Round.Duration < TimeSpan.FromSeconds(60))
			{
				Player.Get(__instance.Owner).ReceiveHint("You cannot yet undisguise");
				return false;
			}
			else
			{
				canStrangle = true;
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Scp3114Strangle))]
	[HarmonyPatch("ValidateTarget")]
	public class Scp3114StranglePatchClass
	{
		[HarmonyPostfix]
		public static void postfix(Scp3114Strangle __instance, ref bool __result, ReferenceHub player)
		{
			if (!Scp3114RevealPatchClass.canStrangle)
			{
				Player.Get(__instance.Owner).ReceiveHint("You must disguise yourself to use Strangulation again!");
				__result = false;
			}

			Scp3114RevealPatchClass.canStrangle = false;

			if (player.playerStats.GetModule<HealthStat>().CurValue > 60)
			{
				Player.Get(__instance.Owner).ReceiveHint("They are still too strong to strangle");
				__result = false;
			}
		}
	}
}
