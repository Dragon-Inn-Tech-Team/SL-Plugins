using CustomCommands.Core;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using System.Collections.Generic;

namespace CustomCommands.Features.DoorLocking
{
	public class DoorLocking : CustomFeatureBase
	{
		public static Dictionary<string, LockType> LockingDict = new Dictionary<string, LockType>();

		public DoorLocking(bool configSetting) : base(configSetting)
		{
			Logger.Info("MEOW??");
		}

		public enum LockType
		{
			Lock, Destroy, NONE
		}

		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev)
		{
			var e = LockingDict.TryGetValue(ev.Player.UserId, out LockType lockType);

			if (e)
			{
				if (ev.Door.Permissions == DoorPermissionFlags.None)
				{
					if (lockType == LockType.Lock)
						ev.IsAllowed = false;
					else if (lockType == LockType.Destroy && ev.Door is BreakableDoor dmgDoor)
						dmgDoor.TryBreak(DoorDamageType.ServerCommand);
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
