namespace CustomCommands.Features.Events.SnowballFight
{
	//	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	//	public class SnowballCommand : ICustomCommand
	//	{
	//		public string Command => "snowballfight";

	//		public string[] Aliases => null;

	//		public string Description => "Enables the christmas-exclusive snowball fight event round";

	//		public string[] Usage { get; } = { };

	//		public PlayerPermissions? Permission => null;
	//		public string PermissionString => "cuscom.events";

	//		public bool RequirePlayerSender => true;

	//		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	//		{
	//			if (!sender.CanRun(this, arguments, out response, out var plrs, out var psender))
	//				return false;

	//			Plugin.CurrentEvent = EventType.SnowballFight;
	//			Round.IsLocked = true;

	//			foreach (Player plr in Player.GetPlayers())
	//			{
	//				plr.SetRole(PlayerRoles.RoleTypeId.ClassD);
	//				plr.Position = new Vector3(130, 996, -43); // Surface zone.
	//				plr.Health = 45;
	//				plr.IsGodModeEnabled = true;
	//				plr.SendBroadcast("The Snowball Fight event is starting! Throw snowballs and be the last one standing. Killing players will restore a small amount of health!", 5);
	//				plr.SendBroadcast("The event will begin in 10 seconds.", 5);
	//				MEC.Timing.CallDelayed(15, () =>
	//				{
	//					plr.AddItem(ItemType.Snowball);
	//					plr.IsGodModeEnabled = false;
	//					plr.SendBroadcast("The Snowball Fight has begun!", 5);
	//				});
	//			}
	//			foreach (var door in DoorVariant.AllDoors)
	//			{
	//				door.NetworkTargetState = false;
	//				door.ServerChangeLock(DoorLockReason.AdminCommand, true);
	//			}


	//			response = "The snowball fight will soon begin!";
	//			return true;
	//		}
	//	}
}
