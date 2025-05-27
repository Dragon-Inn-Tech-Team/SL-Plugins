using CustomCommands.Features.Players.Size;
using CustomPlayerEffects;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp096;
using PlayerStatsSystem;
using RedRightHand;
using RedRightHand.CustomRoles;
using RemoteAdmin.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.CustomRoles.Roles
{
	public class TankRole : CustomRoleBase
	{
		public override string Name => "Heavy Enforcer";

		public override void EnableRole(Player player)
		{
			base.EnableRole(player);

			player.ClearInventory();

			if (player.Team == PlayerRoles.Team.ChaosInsurgency)
				player.AddItem(ItemType.KeycardChaosInsurgency);
			else
				player.AddItem(ItemType.KeycardMTFOperative);

			player.AddItem(ItemType.GunE11SR);
			player.AddItem(ItemType.GunCrossvec);
			player.AddItem(ItemType.ArmorHeavy);
			player.AddItem(ItemType.Radio);
			player.AddItem(ItemType.GrenadeHE);

			player.AddAmmo(ItemType.Ammo9x19, 80);
			player.AddAmmo(ItemType.Ammo556x45, 120);

			player.GetStatModule<StaminaStat>().MaxValue = 0;
			player.MaxHealth = 150;
			player.SetSize(1.15f, 1.15f, 1.15f);
			player.EnableEffect<Slowness>(5, 2000);

			player.GameObject.AddComponent<TankRagdollBoxCollider>();
		}

		public override void DisableRole(Player player)
		{
			base.DisableRole(player);
			player.ResetSize();

			if (player.GameObject.TryGetComponent<TankRagdollBoxCollider>(out var collider))
			{
				UnityEngine.Object.Destroy(collider);
			}
		}
	}

	public class TankRagdollBoxCollider : MonoBehaviour
	{
		public void Awake()
		{
			Player.TryGet(this.gameObject, out var tankPlayer);
			var collider = gameObject.AddComponent<BoxCollider>();
			collider.size = new Vector3(1f, 1f, 1f);
			collider.transform.position = tankPlayer.Camera.position;
		}

		public void Update()
		{
			Player.TryGet(this.gameObject, out var tankPlayer);

			IFpcRole fpcRole = tankPlayer.ReferenceHub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole.FpcModule.SyncMovementState == PlayerMovementState.Sprinting)
			{
				Collider[] hitColliders = new Collider[32];
				int overlap = Physics.OverlapBoxNonAlloc(this.gameObject.transform.position, new Vector3(0.5f, 0.5f, 0.5f), hitColliders);

				for (int i = 0; i < overlap; i++)
				{
					if (Player.TryGet(hitColliders[i].gameObject, out var player) && player.UserId != tankPlayer.UserId && !player.IsSCP && !player.HasEffect<Invisible>())
					{
						player.RagdollPlayer(3, 0.5f, false);
					}
				}
			}	
		}
	}
}
