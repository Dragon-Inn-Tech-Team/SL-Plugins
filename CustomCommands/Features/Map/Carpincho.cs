using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace CustomCommands.Features.Map
{
	public class Carpincho
	{
		[PluginEvent()]
		public void DesovarCarpincho(WaitingForPlayersEvent argumentos)
		{
			if (Scp956Pinata.TryGetInstance(out var capy))
			{
				capy.Network_carpincho = 69;
			}
		}
	}
}
