using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using LabApi.Events;
using LabApi.Events.CustomHandlers;
using RedRightHand.CustomPlugin;
using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using System.Collections.Generic;
using LabApi.Features.Console;

namespace CustomCommands.Features.DoorLocking
{
	public class DoorLocking : CustomFeature
	{
		public static DoorLocking Instance;
		public static Dictionary<string, LockType> LockingDict = new Dictionary<string, LockType>();

		public DoorLocking(bool configSetting) : base(configSetting)
		{
			//Logger.Info("MEOW??");
		}

		public enum LockType
		{
			Lock, Destroy, NONE
		}

		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev)
		{
			if (LockingDict.TryGetValue(ev.Player.UserId, out LockType lockType))
			{
				if (ev.Door.Permissions == DoorPermissionFlags.None)
				{
					if (lockType == LockType.Lock)
						ev.IsAllowed = false;
					else if (lockType == LockType.Destroy && ev.Door is IDamageableDoor dmgDoor)
						dmgDoor.ServerDamage(1000, DoorDamageType.ServerCommand, new Footprinting.Footprint(ev.Player.ReferenceHub));

				}
			}
		}

		public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev)
		{
			if (LockingDict.TryGetValue(ev.Player.UserId, out LockType lockType))
			{
				if (lockType == LockType.Lock)
					ev.IsAllowed = false;
			}
		}
	}
}
