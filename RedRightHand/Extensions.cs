using CommandSystem;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.MicroHID;
using LabApi.Features.Console;
using LabApi.Features.Permissions;
using LabApi.Features.Permissions.Providers;
using LabApi.Features.Wrappers;
using Newtonsoft.Json.Linq;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using RedRightHand.Commands;
using RedRightHand.CustomPlugin;
using RedRightHand.DataStores;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using Utils;
using Logger = LabApi.Features.Console.Logger;

namespace RedRightHand
{
	public static class Extensions
	{
		public static bool RoundInProgress() => Round.IsRoundStarted && !Round.IsRoundEnded;

		public static bool CanRun(this ICommandSender sender, ICustomCommand cmd, ArraySegment<string> args, out string Response, out List<Player> Players, out PlayerCommandSender PlrCmdSender)
		{
			Players = [];
			PlrCmdSender = null;

			if (cmd.RequirePlayerSender)
			{
				if (sender is not PlayerCommandSender pSender)
				{
					Response = "You must be a player to run this command";
					return false;
				}
				PlrCmdSender = pSender;
			}

			if (cmd.Permission != null && !sender.CheckPermission((PlayerPermissions)cmd.Permission))
			{
				Response = $"You do not have the required permission to execute this command: {cmd.Permission}";
				return false;
			}
			else if (!string.IsNullOrEmpty(cmd.PermissionString) && !CheckPermission(sender, cmd.PermissionString))
			{
				Response = $"You do not have the required permission to execute this command: {cmd.PermissionString}";
				return false;
			}

			if (cmd.Usage != null)
			{
				if (args.Count < cmd.Usage.Length)
				{
					Response = $"Missing argument: {cmd.Usage[args.Count]}";
					return false;
				}

				if (cmd.Usage.Contains("%player%"))
				{
					var index = cmd.Usage.IndexOf("%player%");
					string text = args.ElementAt(index);

					if (text.Equals("*") || text.Equals("all"))
					{
						foreach (var plr in Player.GetAll())
						{
							Players.Add(plr);
						}
					}
					else
					{
						var hubs = RAUtils.ProcessPlayerIdOrNamesList(args, index, out _, false);

						if (hubs.Count < 1)
						{
							Response = $"No player(s) found for: {args.ElementAt(index)}";
							return false;
						}
						else
						{
							foreach (var plr in hubs)
							{
								Players.Add(Player.Get(plr));
							}
						}
					}



				}
			}

			Response = string.Empty;
			return true;
		}

		public static bool TryGetCommandArgument<T>(this ArraySegment<string> args, int position, T defaultOutput, out T? argument, out string response)
		{
			response = string.Empty;
			if (args.Count >= position + 1)
			{
				try
				{
					argument = (T)Convert.ChangeType(args.ElementAt(position), typeof(T));
					return true;
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
					response = $"Unable to convert argument at postition {position} to type {typeof(T).Name}";
				}
			}
			else
				response = $"Too few arguments provided";


			argument = defaultOutput;
			return false;
		}

		public static bool CheckPermission(ICommandSender sender, string permission)
		{
			if (sender is PlayerCommandSender pCS)
			{
				var plr = Player.Get(pCS);

				if (plr != null)
				{
					return plr.HasPermissions(permission);
				}
				//Return false if player cannot be found. How was the command run in the first place?
				else return false;
			}
			else return true;
		}

		public static RoleTypeId GetRoleFromString(string role)
		{
			if (Enum.TryParse(role, true, out RoleTypeId roleType))
			{
				if (!IsValidSCP(roleType))
					return RoleTypeId.None;

				return roleType;
			}
			else return RoleTypeId.None;
		}

		private static readonly RoleTypeId[] _isCoreSCP =
		[
			RoleTypeId.Scp173, RoleTypeId.Scp049, RoleTypeId.Scp079,RoleTypeId.Scp096, RoleTypeId.Scp106,RoleTypeId.Scp939, RoleTypeId.Scp3114
		];

		public static bool IsValidSCP(this RoleTypeId role)
		{
			return _isCoreSCP.Contains(role);
		}

		public static string SCPNumbersFromRole(this RoleTypeId role)
		{
			if (IsValidSCP(role))
			{
				return role.ToString().ToLower().Replace("scp", "");
			}
			else return string.Empty;
		}

		public static void AddToOrReplaceValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
				dict[key] = value;
			else
				dict.Add(key, value);
		}
		public static void AddToOrUpdateValue<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
		{
			if (dict.ContainsKey(key))
				dict[key] += value;
			else
				dict.Add(key, value);
		}

		public static void UpdatePrivateProperty(this object obj, string propName, object propValue, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
		{
			var propinfo = obj.GetType().GetProperty(propName, flags);
			propinfo.SetValue(obj, propValue);
		}

		public static string GetDamageSource(this AttackerDamageHandler aDH)
		{
			if (aDH is FirearmDamageHandler fDH)
				return fDH.WeaponType.ToString();
			else if (aDH is ExplosionDamageHandler)
				return "Grenade";
			else if (aDH is MicroHidDamageHandler)
				return "Micro HID";
			else if (aDH is RecontainmentDamageHandler)
				return "Recontainment";
			else if (aDH is Scp018DamageHandler)
				return "SCP 018";
			else if (aDH is Scp096DamageHandler)
				return "SCP 096";
			else if (aDH is Scp049DamageHandler)
				return "SCP 049";
			else if (aDH is Scp939DamageHandler)
				return "SCP 939";
			else if (aDH is Scp3114DamageHandler)
				return "SCP 3114";
			else if (aDH is ScpDamageHandler scpDH)
				return scpDH.Attacker.Role.ToString();
			else if (aDH is DisruptorDamageHandler)
				return "Particle Disruptor";
			else if (aDH is JailbirdDamageHandler)
				return "Jailbird";
			else return $"{aDH.GetType().Name}";
		}

		public static string ToLogString(this Player plr) => $"{plr.Nickname} ({plr.UserId})";

		public static bool IsChaos(this Player player, bool includeCivs = false) => IsChaos(player.Role, includeCivs);
		public static bool IsChaos(this RoleTypeId player, bool includeCivs = false)
		{
			switch (player)
			{
				case RoleTypeId.ChaosConscript:
				case RoleTypeId.ChaosRifleman:
				case RoleTypeId.ChaosRepressor:
				case RoleTypeId.ChaosMarauder:
					return true;
				case RoleTypeId.ClassD:
					if (includeCivs)
						return true;
					else return false;
				default:
					return false;
			}
		}
		public static bool IsMtf(this Player player, bool includeCivs = false) => IsMtf(player.Role, includeCivs);
		public static bool IsMtf(this RoleTypeId player, bool includeCivs = false)
		{
			switch (player)
			{
				case RoleTypeId.FacilityGuard:
				case RoleTypeId.NtfCaptain:
				case RoleTypeId.NtfSpecialist:
				case RoleTypeId.NtfPrivate:
				case RoleTypeId.NtfSergeant:
					return true;
				case RoleTypeId.Scientist:
					if (includeCivs)
						return true;
					else return false;
				default:
					return false;
			}
		}

		public static bool IsSCP(this Player player)
		{
			return player.Role switch
			{
				RoleTypeId.Scp173 or RoleTypeId.Scp106 or RoleTypeId.Scp049 or RoleTypeId.Scp079 or RoleTypeId.Scp096 or RoleTypeId.Scp0492 or RoleTypeId.Scp939 or RoleTypeId.Scp3114 => true,
				_ => false,
			};
		}

		public static bool IsFF(this Player victim, Player Attacker, bool CheckStore = false)
		{
			var victimRole = victim.ReferenceHub.roleManager.CurrentRole;
			var AttackerRole = Attacker.ReferenceHub.roleManager.CurrentRole;

			if (victimRole.Team == Team.SCPs || AttackerRole.Team == Team.SCPs)
				return false;

			if (IsChaos(victim, true) && IsChaos(Attacker, true))
			{
				if (victim.Role == RoleTypeId.ClassD && Attacker.Role == RoleTypeId.ClassD)
					return false;
				return true;
			}
			else if (IsMtf(victim, true) && IsMtf(Attacker, true))
				return true;

			if (CheckStore)
			{
				var ffdStore = Attacker.GetDataStore<FFDStore>();

				if (ffdStore.PreviousRole == RoleTypeId.None)
					return false;

				if (IsChaos(victim, true) && IsChaos(ffdStore.PreviousRole, true))
				{
					if (victim.Role == RoleTypeId.ClassD && ffdStore.PreviousRole == RoleTypeId.ClassD)
						return false;
					return true;
				}
				else if (IsMtf(victim, true) && IsMtf(ffdStore.PreviousRole, true))
					return true;
			}

			return false;
		}

		public async static Task<HttpResponseMessage> Post(string Url, StringContent Content)
		{
			using HttpClient client = new();
			client.BaseAddress = new Uri(Url);

			return await client.PostAsync(client.BaseAddress, Content);
		}
		public async static Task<HttpResponseMessage> Get(string Url)
		{
			using HttpClient client = new();
			client.BaseAddress = new Uri(Url);

			return await client.GetAsync(client.BaseAddress);
		}

		public static bool TryParseJSON(string json, out JObject jObject)
		{
			try
			{
				jObject = JObject.Parse(json);
				return true;
			}
			catch
			{
				jObject = null;
				return false;
			}
		}

		public static char[] ValidDurationUnits = ['m', 'h', 'd', 'w', 'M', 'y'];
		public static TimeSpan GetBanDuration(char unit, int amount)
		{
			return unit switch
			{
				'h' => new TimeSpan(0, amount, 0, 0),
				'd' => new TimeSpan(amount, 0, 0, 0),
				'w' => new TimeSpan(7 * amount, 0, 0, 0),
				'M' => new TimeSpan(30 * amount, 0, 0, 0),
				'y' => new TimeSpan(365 * amount, 0, 0, 0),
				_ => new TimeSpan(0, 0, amount, 0),
			};
		}
		public static bool TryGetBanDuration(string durationString, out TimeSpan duration)
		{
			duration = TimeSpan.Zero;

			var chars = durationString.Where(Char.IsLetter).ToArray();
			if (chars.Length < 1 || !int.TryParse(new string(durationString.Where(Char.IsDigit).ToArray()), out int amount) || !Extensions.ValidDurationUnits.Contains(chars[0]) || amount < 1)
				return false;

			duration = Extensions.GetBanDuration(chars[0], amount);
			return true;
		}

		public static bool TryFindPlayer(string searchParam, out Player plr)
		{
			plr = null;

			var plrs = Player.List;
			IEnumerable<Player> posPlrs;

			if (searchParam.Contains('@'))
			{
				posPlrs = plrs.Where(p => p.UserId == searchParam).ToArray();

				if (posPlrs.Any())
				{
					plr = posPlrs.First();
					return true;
				}
			}
			else if (IPAddress.TryParse(searchParam, out IPAddress IP))
			{
				posPlrs = plrs.Where(p => p.IpAddress == searchParam).ToArray();

				if (posPlrs.Any())
				{
					plr = posPlrs.First();
					return true;
				}
			}

			posPlrs = plrs.Where(p => p.Nickname.ToLowerInvariant() == searchParam.ToLowerInvariant()).ToArray();

			if (posPlrs.Any())
			{
				plr = posPlrs.First();
				return true;
			}

			return false;
		}

		public static List<Player> GetNearbyPlayers(Player player, bool rangeOnly = false)
		{
			float distanceCheck = player.Position.y > 900 ? 70 : 35;
			List<Player> nearbyPlayers = [];

			foreach (var plr in Player.List)
			{
				if (plr.IsHost || plr.Role == RoleTypeId.Spectator)
					continue;

				var distance = Vector3.Distance(player.Position, plr.Position);

				if (rangeOnly && distance <= distanceCheck)
					nearbyPlayers.Add(plr);
				else
				{
					var angle = Vector3.Angle(player.GameObject.transform.forward, player.Position - plr.Position);

					if ((distance <= distanceCheck && angle > 130) || distance < 5)
						nearbyPlayers.Add(plr);
				}
			}

			return nearbyPlayers;
		}

		/// <summary>
		/// Mostly used for Nina's AI stuffs. Checks stats, class and inventory to give players a threat level. Higher is more of a threat, lower is less of a threat.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static int GetPlayerThreatLevel(Player player)
		{
			int level = 1;

			//Checks if the player is MTF or chaos (not including dclass or scientists) and adds 10 threat if they are.
			if (player.IsMtf() || player.IsChaos())
				level += 10;

			//Checks what item the player is currently holding. Any special weapon will add 10 threat, normal weapons, grenades and SCP items add 5 threat, and medical items add 3.
			if (Item.TryGet(player.CurrentItem.Serial, out var item))
			{
				if (item.Category == ItemCategory.SpecialWeapon)
					level += 10;
				else if (item.Category == ItemCategory.Firearm || item.Category == ItemCategory.SCPItem || item.Category == ItemCategory.Grenade)
					level += 5;
				else if (item.Category == ItemCategory.Medical)
					level += 3;
			}

			//Multiplies their threat level based on their current HP
			level *= (int)Math.Round(Mathf.Clamp(player.Health / player.MaxHealth, 0.2f, 1.25f), 0);

			return level;
		}
	}

	public class FakeRagdoll : MonoBehaviour
	{

	}
}
