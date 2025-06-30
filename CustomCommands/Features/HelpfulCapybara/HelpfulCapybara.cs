using AdminToys;
using CustomCommands.Core;
using CustomCommands.Features.SurfaceLightFix;
using Hints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.DedicatedServer;
using static System.Net.Mime.MediaTypeNames;
using CapybaraToy = LabApi.Features.Wrappers.CapybaraToy;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.HelpfulCapybara
{
	public class HelpfulCapybara : CustomFeature
	{
		public static RoomShape[] CapybaraRoomShapes =
		[
			RoomShape.Straight, RoomShape.Curve, RoomShape.TShape, RoomShape.XShape
		];

		public static FacilityZone CapybaraZone = FacilityZone.Entrance;
		public static Vector3 CapybaraPosition = new Vector3(63.068f, 290.6f, -51f);

		public static HelpfulCapybaraTools Capybara;

		public HelpfulCapybara(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerRoundStarted()
		{
			var interactToy = InteractableToy.Create(CapybaraPosition, null, true);
			interactToy.GameObject.AddComponent<HelpfulCapybaraTools>();
			interactToy.InteractionDuration = 5;
			interactToy.Scale = new Vector3(2, 2, 2);
			interactToy.GameObject.GetComponent<HelpfulCapybaraTools>().Capybara = CapybaraToy.Create(CapybaraPosition, null, true);
			Logger.Info("InteractableToy Spawned");

			Capybara = interactToy.GameObject.GetComponent<HelpfulCapybaraTools>();
		}

		public override void OnPlayerSearchedToy(PlayerSearchedToyEventArgs ev)
		{
			if (ev.Interactable.GameObject.TryGetComponent<HelpfulCapybaraTools>(out var hCT))
			{
				if (hCT.WillingToHelp(ev.Player))
				{
					var room = Map.Rooms.Where(r => r.Zone == CapybaraZone && r.Name == RoomName.EzRedroom);
					if (room.Any())
					{
						var index = UnityEngine.Random.Range(0, room.Count() - 1);

						//Selects all friendly players within a 3 meter radius, and teleports the player and up to 3 friendlies to the room the capybara has selected.
						var group = Player.GetAll(LabApi.Features.Enums.PlayerSearchFlags.AuthenticatedPlayers)
							.Where(p => Vector3.Distance(p.Position, ev.Player.Position) < 3 && p.Team == ev.Player.Team && p.UserId != ev.Player.UserId && !p.Items.Where(i => i.Type == ItemType.Radio).Any()).ToList();
						ev.Player.Position = room.ElementAt(index).Position + Vector3.up * 1;
						if (group.Any())
							for (int i = 0; i < group.Count(); i++)
								if (i < 4)
									group.ElementAt(i).Position = room.ElementAt(index).Position + Vector3.up * 1;

						hCT.LastUsed = Round.Duration.Add(new TimeSpan(0, 0, hCT.CooldownSeconds));
						hCT.RemainingUses -= 1;
					}
				}
				else
					ev.Player.SendHint("The capybara ignores you");
			}
		}

		public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
		{
			Capybara.RemainingUses = 1;
			Capybara.HelpfulTowards = ev.Wave.Faction;
			Capybara.LastUsed = Capybara.LastUsed > Round.Duration ? Capybara.LastUsed.Add(new TimeSpan(0, 1, 0)) : Round.Duration.Add(new TimeSpan(0, 1, 0));
		}
	}

	public class HelpfulCapybaraTools : MonoBehaviour
	{
		public int CooldownSeconds = 60;
		public TimeSpan LastUsed = new TimeSpan(0, 0, 0);
		public int RemainingUses = 1;
		public CapybaraToy Capybara;
		public Faction HelpfulTowards = Faction.Flamingos;
		public InteractableToy Interactable => gameObject.GetComponent<InteractableToy>();

		public bool WillingToHelp(Player plr)
		{
			return plr.Health == plr.MaxHealth && Round.Duration > LastUsed && RemainingUses > 0 && !Warhead.IsDetonated && !Warhead.IsDetonationInProgress && !plr.Items.Where(i => i.Type == ItemType.Radio).Any();
		}
	}
}
