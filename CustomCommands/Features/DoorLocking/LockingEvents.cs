using Interactables.Interobjects.DoorUtils;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace CustomCommands.Features.DoorLocking
{
	public class LockingEvents
	{
		[PluginEvent, PluginPriority(LoadPriority.Highest)]
		public bool OnPlayerDoorInteract(PlayerInteractDoorEvent args)
		{

			if (args.Player.TemporaryData.Contains("plock") && args.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
			{
				args.Door.ServerChangeLock(DoorLockReason.AdminCommand, true);
				args.Door.UnlockLater(1, DoorLockReason.AdminCommand);

				args.CanOpen = false;
				return false;
			}
			else if (args.Player.TemporaryData.Contains("pdest") && args.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
			{
				if (args.Door is IDamageableDoor dmgDoor && args.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
				{
					dmgDoor.ServerDamage(10000, DoorDamageType.ServerCommand);

					args.CanOpen = false;
					return false;
				}
			}

			return args.CanOpen;
		}

		[PluginEvent]
		public bool PlayerInteractElevator(PlayerInteractElevatorEvent args)
		{
			if (args.Player.TemporaryData.Contains("plock"))
			{
				return false;
			}

			return true;
		}
	}
}
