using InventorySystem.Items.Pickups;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector
{
	public class FFInfo
	{
		public FFInfo()
		{
			LastUpdate = DateTime.Now;
		}
		public string PlayerId { get; set; }
		public int Value { get; set; }
		public DateTime LastUpdate { get; set; }
	}
	public class GrenadeThrowerInfo
	{
		public string PlayerId { get; set; }
		public bool HostilesNearby { get; set; }
		public bool HostilesNearExplosion { get; set; }

		public RoleTypeId Role { get; set; }

		public ushort Serial { get; set; }
		public DateTime Explosion { get; set; }
	}
}
