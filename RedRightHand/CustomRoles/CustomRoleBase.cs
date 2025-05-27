using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RedRightHand;

namespace RedRightHand.CustomRoles
{
	public abstract class CustomRoleBase
	{
		private string teamToNiceName(Team team)
		{
			switch (team)
			{
				case Team.SCPs:
					return "SCP";
				case Team.FoundationForces:
				case Team.Scientists:
					return "Nine-Tailed Fox";
				case Team.ChaosInsurgency:
					return "Chaos";
				case Team.ClassD:
					return "Class D";
				case Team.Dead:
				case Team.OtherAlive:
				case Team.Flamingos:
				default:
					return "OtherBeings";
			}
		}

		public abstract string Name { get; }

		public virtual void EnableRole(Player player)
		{
			player.CustomInfo = $"{teamToNiceName(player.Team)} - {Name}";
		}
		public virtual void DisableRole(Player player)
		{
			player.CustomInfo = string.Empty;
		}





		public virtual void PlayerHurt(PlayerHurtingEventArgs ev) { }
		public virtual void PlayerDied(PlayerDeathEventArgs ev)
		{
			CustomRolesManager.DisableRole(ev.Player);
		}

		public virtual void PlayerChangeRole(PlayerChangedRoleEventArgs ev)
		{
			CustomRolesManager.DisableRole(ev.Player);
		}
	}
}
