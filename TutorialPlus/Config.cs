using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialPlus
{
	public class Config
	{
		[Description("Should tutorials be able to trigger SCP-096 and become a target")]
		public bool TutorialTrigger096 { get; set; } = false;
		[Description("Should tutorials be able to freeze SCP-173 and become an observer")]
		public bool TutorialObserve173 { get; set; } = false;

		[Description("Should tutorials be able to be handcuffed")]
		public bool CuffableTutorial { get; set; } = false;

		[Description("Automatically enable godmode when a player spawns as tutorial (Will also automatically disable if the player changes role)")]
		public bool TutorialGodmode { get; set; } = true;
		[Description("Automatically enable bypass mode when a player spawns as tutorial (Will also automatically disable if the player changes role)")]
		public bool TutorialBypass { get; set; } = true;
		[Description("Automatically enable noclip when a player spawns as tutorial (Will also automatically disable if the player changes role)")]
		public bool TutorialNoclip { get; set; } = true;

		[Description("Enables debug messages in the server console")]
		public bool Debug { get; set; } = false;
	}
}
