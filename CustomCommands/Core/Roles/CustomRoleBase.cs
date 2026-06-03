using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RedRightHand;
using static CustomCommands.Features.Custom.Roles.CustomRolesManager;

namespace CustomCommands.Core.Roles
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

		public abstract CustomRoleType CustomRole { get; }
		public abstract string Name { get; }

		public virtual void EnableRole(Player player)
		{
			player.CustomInfo = $"{teamToNiceName(player.Team)} - {Name}";
			ActiveRoles.AddToOrReplaceValue(player.UserId, CustomRole);
		}

		public virtual void DisableRole(Player player)
		{
			player.CustomInfo = string.Empty;
			ActiveRoles.Remove(player.UserId);
		}

		public virtual bool ToggleRole(Player player)
		{
			if (ActiveRoles.ContainsKey(player.UserId))
			{
				DisableRole(player);
				return false;
			}
			else
			{
				EnableRole(player);
				return true;
			}
		}

		public virtual void PlayerHurt(PlayerHurtingEventArgs ev) { }
		public virtual void PlayerDied(PlayerDeathEventArgs ev)
		{
			DisableRole(ev.Player);
		}

		public virtual void PlayerChangeRole(PlayerChangedRoleEventArgs ev)
		{
			DisableRole(ev.Player);
		}
	}
}
