using RedRightHandCore.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedGameplay
{
	public class EnhancedGameplayPluginCore : CustomPluginCore
	{
		public override string Name => "Enhanced Gameplay";

		public override string Description => "A bunch of misc. changes made to gameplay for our servers";

		public override Version Version => new(0, 0, 0, 1);

		public override void Disable()
		{
			throw new NotImplementedException();
		}

		public override void Enable()
		{
			throw new NotImplementedException();
		}
	}
}
