using AdminToys;
using Interactables;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using Mirror;
using RedRightHand;
using RedRightHand.CustomWeapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Features.CustomWeapons.Weapons
{
	public class CapybaraGun : CustomWeaponBase
	{
		public override string Name => "CapybaraGun";
		public override ItemType Model => ItemType.GunCom45;

		public override void PlaceBulletHole(PlayerPlacedBulletHoleEventArgs ev)
		{
			var hasToy = NetworkClient.prefabs.Values.Where(r => r.TryGetComponent<AdminToyBase>(out var toyBase) && string.Equals(toyBase.CommandName, "capybara", StringComparison.InvariantCultureIgnoreCase));

			if (hasToy.Any())
			{
				var toy = hasToy.First().GetComponent<AdminToyBase>();
				GameObject capy = UnityEngine.Object.Instantiate(toy).gameObject;
				capy.transform.position = ev.HitPosition;
				capy.transform.LookAt(ev.Player.Position);
				NetworkServer.Spawn(capy);
			}
		}
	}
}
