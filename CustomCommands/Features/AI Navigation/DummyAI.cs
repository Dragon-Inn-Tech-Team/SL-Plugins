using CustomCommands.Features.Testing.Navigation;
using Interactables;
using Interactables.Verification;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using Mirror;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RedRightHand.Navigation.NavMeshComponents;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.TestingFeatures
{
	public class DummyAI : MonoBehaviour
	{
		private ReferenceHub _hub;
		private NavMeshAgent _agent;
		private float _speed;
		private float _lastPlrCheck = 0;
		private KeyValuePair<Player, int> _highestThreatPlayer = new();
		IFpcRole FpcRole => _hub.roleManager.CurrentRole as IFpcRole;
		FirstPersonMovementModule FPCModule => FpcRole.FpcModule;
		Dictionary<byte, double> interactCooldown = new Dictionary<byte, double>();

		public void Init(ReferenceHub hub, NavMeshAgent agent, float speed = 30f)
		{
			_hub = hub;
			_agent = agent;
			_speed = speed;
		}

		private void FixedUpdate()
		{
			if (NetworkServer.active && _hub.isActiveAndEnabled && _hub.IsAlive())
			{
				if (!_agent.isOnNavMesh)
				{
					Logger.Info("Not on nav mesh");

					if (_hub.TryGetCurrentRoom(out var room))
					{
						var e = room.gameObject.TryGetComponent<NavMeshSurface>(out var nvs);

						if (nvs != null)
							nvs.BuildNavMesh();

						else NavigationEvents.buildNavMesh(room.gameObject);
					}
				}

				KeyValuePair<float, Player> closestPlayer = new KeyValuePair<float, Player>(500, null);

				var players = RedRightHand.Extensions.GetNearbyPlayers(Player.Get(_hub), true);

				foreach (var p in players)
				{
					if (p.PlayerId == _hub.PlayerId)
						continue;

					var dist = Vector3.Distance(_hub.GetPosition(), p.Position);

					if (dist < closestPlayer.Key)
						closestPlayer = new KeyValuePair<float, Player>(dist, p);
				}

				FpcRole.LookAtPoint(closestPlayer.Value.Position + (Vector3.up * 0.5f));
				NavigationManager.SetDestination(closestPlayer.Value.GameObject.transform.position, _agent);

				//Logger.Debug($"{closestPlayer.Value.GameObject.transform.position} {closestPlayer.Value.Position}");

				//Logger.Info(closestPlayer.Key);

				if (closestPlayer.Key > 15 && closestPlayer.Key < 30)
				{
					var dummyPlayer = Player.Get(_hub);

					if (dummyPlayer.CurrentItem is FirearmItem firearm)
					{
						if (firearm.ChamberedAmmo > 0)
							DummyActionCollector.ServerGetActions(_hub).Where(r => r.Name == "Shoot->Click").First().Action();
						else
							DummyActionCollector.ServerGetActions(_hub).Where(r => r.Name == "Reload->Click").First().Action();
					}
				}


				//if (!_agent.isOnNavMesh)
				//{
				//	_agent.enabled = false;
				//	_agent.enabled = true;
				//	return;
				//}



				//if (Physics.Raycast(_hub.PlayerCameraReference.position, _hub.transform.forward, out var hitInfo, 1.5f, InteractionCoordinator.RaycastMask) && !_agent.isStopped)
				//{
				//	_agent.isStopped = true;

				//	MEC.Timing.RunCoroutine(interact(hitInfo));
				//}

				////if (!_agent.isStopped)
				////Logger.Info($"{_agent.destination} {_agent.remainingDistance}");

				//IFpcRole fpcRole = _hub.roleManager.CurrentRole as IFpcRole;
				//if (fpcRole != null)
				//{
				//	if (_lastPlrCheck > 50)
				//	{
				//		_lastPlrCheck = 0;

				//		//Logger.Info("Checking Threat");
				//		var players = RedRightHand.Extensions.GetNearbyPlayers(Player.Get(_hub), true);

				//		foreach (var p in players)
				//		{
				//			CheckThreat(p);
				//		}

				//		SetDestination(_highestThreatPlayer.Key.Position);
				//	}
				//	_lastPlrCheck++;


				//	FirstPersonMovementModule fpcModule = fpcRole.FpcModule;
				//	Vector3 pos = _hub.transform.position;
				//	var dist = _agent.remainingDistance;
				//	if (dist < _agent.stoppingDistance)
				//		_agent.ResetPath();

				//	if (dist > 1 && dist < 300)
				//	{
				//		Vector3 position = base.transform.position;
				//		Vector3 dir = _agent.destination - position;
				//		Vector3 b = Time.deltaTime * this._speed * dir.normalized;
				//		fpcModule.MouseLook.LookAtDirection(dir, 1f);
				//	}
				//	else if (dist > 300)
				//		_agent.ResetPath();

				//	return;
				//}
			}
			else
			{
				Destroy(this);
			}
		}

		IEnumerator<float> interact(RaycastHit hitInfo)
		{
			if (hitInfo.collider.TryGetComponent(out InteractableCollider interCollider) && (interCollider is IInteractable iInter))
			{
				if (!interactOnCooldown(interCollider.ColliderId) && (interCollider.Target is NetworkBehaviour netBehav) && (netBehav.TryGetComponent(out IServerInteractable iServerInter)))
				{

					bool canInteract = GetSafeRule(iInter).ClientCanInteract(interCollider, hitInfo);

					if (canInteract)
					{
						iServerInter.ServerInteract(_hub, interCollider.ColliderId);
						interactCooldown.Add(interCollider.ColliderId, Round.Duration.TotalSeconds); //Stop the AI just spam interacting

						yield return Timing.WaitForSeconds(1f);
					}

					_agent.isStopped = false;
					yield return 0f;
				}
			}
		}

		private void lookAt(Vector3 position)
		{
			FPCModule.MouseLook.LookAtDirection((position - _hub.GetPosition().normalized), 1f);
		}

		private bool interactOnCooldown(byte colliderID)
		{
			return interactCooldown.ContainsKey(colliderID) && Round.Duration.TotalSeconds - interactCooldown[colliderID] < 1.5;
		}

		private static IVerificationRule GetSafeRule(IInteractable inter)
		{
			return inter.VerificationRule ?? StandardDistanceVerification.Default;
		}

		public void CheckThreat(Player player)
		{
			var threat = RedRightHand.Extensions.GetPlayerThreatLevel(player);

			//Logger.Info($"Threat: CUR {_highestThreatPlayer.Value} CHECK {threat}");

			if (threat > _highestThreatPlayer.Value)
			{
				//Logger.Info($"New Threat: {player.Nickname}");
				_highestThreatPlayer = new KeyValuePair<Player, int>(player, threat);
			}
		}
	}
}
