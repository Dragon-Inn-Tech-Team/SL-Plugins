﻿using Interactables.Verification;
using Interactables;
using Mirror;
using PlayerRoles.FirstPersonControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.TestingFeatures
{
	public class DummyAI : MonoBehaviour
	{
		private ReferenceHub _hub;
		private NavMeshAgent _agent;
		private float _speed;

		public void Init(ReferenceHub hub, NavMeshAgent agent, float speed = 30f)
		{
			_hub = hub;
			_agent = agent;
			_speed = speed;
		}

		private void Update()
		{
			if (NetworkServer.active)
			{
				if (!_agent.isOnNavMesh)
				{
					_agent.enabled = false;
					_agent.enabled = true;
					return;
				}

				//if (!_agent.isStopped)
				//Logger.Info($"{_agent.destination} {_agent.remainingDistance}");

				if (Physics.Raycast(_hub.PlayerCameraReference.position, _hub.transform.forward, out var hitInfo, 1f, InteractionCoordinator.InteractRaycastMask))
				{
					Logger.Info($"AAA {hitInfo.collider.name}");
					interact(hitInfo);
				}

				IFpcRole fpcRole = _hub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					FirstPersonMovementModule fpcModule = fpcRole.FpcModule;
					Vector3 pos = _hub.transform.position;
					var dist = _agent.remainingDistance;
					if(dist < _agent.stoppingDistance)
						_agent.ResetPath();

					if (dist > 1 && dist < 300)
					{
						Vector3 position = base.transform.position;
						Vector3 dir = _agent.destination - position;
						Vector3 b = Time.deltaTime * this._speed * dir.normalized;
						fpcModule.MouseLook.LookAtDirection(dir, 1f);
					}
					else if (dist > 300)
						_agent.ResetPath();

					return;
				}
			}

			Destroy(this);
		}

		void interact(RaycastHit hitInfo)
		{
			bool hasIntCol = hitInfo.collider.TryGetComponent<InteractableCollider>(out var intCol);
			bool isInteractable = intCol.Target is IInteractable;
			bool canInteract = GetSafeRule(intCol.Target as IInteractable).ClientCanInteract(intCol, hitInfo);
			bool isNetworkBehaviour = intCol.Target is NetworkBehaviour;
			bool hasServercompo = (intCol.Target as NetworkBehaviour).TryGetComponent<IServerInteractable>(out var serverCompo);

			Logger.Info($"{hasIntCol} {isInteractable} {canInteract} {isNetworkBehaviour} {hasServercompo}");

			if (!hasIntCol || !isInteractable ||
				 !canInteract)
				return;

			Logger.Info($"bbbb");

			serverCompo.ServerInteract(_hub, intCol.ColliderId);
		}

		private static IVerificationRule GetSafeRule(IInteractable inter)
		{
			return inter.VerificationRule ?? StandardDistanceVerification.Default;
		}

		public void SetDestination(Vector3 target)
		{
			_agent.ResetPath();
			_agent.SetDestination(target);
			Logger.Debug($"New Destination: {_agent.destination} {_agent.remainingDistance} {_agent.pathStatus} {_agent.path.status} {_agent.isOnNavMesh}");
		}
	}
}
