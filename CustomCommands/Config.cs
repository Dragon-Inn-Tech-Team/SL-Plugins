namespace CustomCommands
{
	public class Config
	{
		//Enables the plock and pdestroy commands
		public bool EnableDoorLocking { get; set; } = true;

		//Enables dummy players
		public bool EnableDummies { get; set; } = false;
		//Requires dummies to be enabled
		public bool EnableSteve { get; set; } = false;

		//Enables the events such as TDM and infection
		public bool EnableEvents { get; set; } = true;

		//Enables the weekly events
		public bool EnableWeeklyEvents { get; set; } = false;

		//Randomizes the delay for the nuke's DeadManSwitch
		public bool EnableDmsRandomizedDelay { get; set; } = true;

		//Enables the better disarming system (rewards more tokens for disarming and rescuing dclass and scientists)
		public bool EnableBetterDisarming { get; set; } = true;

		//Enables the late join system
		public bool EnableLateJoin { get; set; } = true;
		public int LateJoinTime { get; set; } = 30;

		//Enables the late spawn system
		public bool EnableLateSpawn { get; set; } = true;
		public int LateSpawnTime { get; set; } = 10;

		//Enables the tutorial fixes (blocks handcuffing)
		public bool EnableTutorialFixes { get; set; } = true;
		//Should tutorials explode when a flipped coin lands on tails
		public bool TutorialCoinExplosion { get; set; } = false;

		//Enables the special weapons.
		public bool EnableSpecialWeapons { get; set; } = false;
		public bool EnableGrenadeLauncher { get; set; } = true;
		public bool EnableFlashbangLauncher { get; set; } = true;
		public bool EnableBallLauncher { get; set; } = true;
		public bool EnableRagdollLauncher { get; set; } = true;
		public bool EnableTranqGun { get; set; } = true;


		public bool EnableAdditionalSurfaceLighting { get; set; } = true; //Enables extra surface lights
		public bool EnableDamageAnnouncements { get; set; } = true; //Enables the SCP damage announcements at the end of the round
		public bool EnableScp079Removal { get; set; } = true; //Enables the SCP-079 removal (Yes it's confusing, but it's being kept as Enable to keep configs consistant)
		public bool EnableScpSwap { get; set; } = true; //Enables the SCP Swap system
		public bool EnableDebugTests { get; set; } = true; //Enables the debug tests
		public bool EnablePlayerVoting { get; set; } = true; //Enables player voting
		public bool EnableBlackout { get; set; } = true; //Enables the blackouts
		public int MinBlackoutTime { get; set; } = 180; //Minimum time between round start and blackout
		public int MaxBlackoutTime { get; set; } = 360; //Maximum time between round start and blackout
		public int BlackoutDuration { get; set; } = 30; //Duration of the blackout 
	}
}
