using Mirror;
using PluginAPI.Core;
using System.Reflection;

namespace CustomCommands.Features.Players.Size
{
	public static class SizeManager
	{
		public static void SetSize(this Player plr, int x, int y, int z)
		{
			var svrPlrs = Server.GetPlayers();

			var nId = plr.ReferenceHub.networkIdentity;
			plr.ReferenceHub.gameObject.transform.localScale = new UnityEngine.Vector3(1 * x, 1 * y, 1 * z);

			foreach (var player in svrPlrs)
			{
				NetworkConnection nConn = player.ReferenceHub.connectionToClient;

				typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { nId, nConn });
			}
		}

		public static void ResetSize(this Player plr)
		{
			var svrPlrs = Server.GetPlayers();

			var nId = plr.ReferenceHub.networkIdentity;
			plr.ReferenceHub.gameObject.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

			foreach (var player in svrPlrs)
			{
				NetworkConnection nConn = player.ReferenceHub.connectionToClient;

				typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { nId, nConn });
			}
		}
	}
}
