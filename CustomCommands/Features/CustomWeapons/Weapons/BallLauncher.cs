﻿using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomWeapons.Weapons
{
	public class BallLauncher : CustomWeaponBase
	{
		public override CustomWeapons.WeaponType WeaponType => CustomWeapons.WeaponType.Ball;

		public override string Name => "SCP-018 Launcher";

		public override void ShootWeapon(Player player)
		{
			Helpers.SpawnGrenade<Scp018Projectile>(player, ItemType.SCP018, Helpers.RandomThrowableVelocity(player.Camera.transform));
		}
	}
}
