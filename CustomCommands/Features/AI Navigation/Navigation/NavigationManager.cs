
using CustomCommands.Core;
using UnityEngine;
using UnityEngine.AI;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.Testing.Navigation
{
	public class NavigationManager : CustomFeatureBase
	{
		public NavigationManager(bool configSetting) : base(configSetting)
		{
		}

		public static void SetDestination(Vector3 target, NavMeshAgent agent)
		{
			target += Vector3.up * 0.5f;
			DrawableLine.DrawableLines.ServerGenerateLine(Color.cyan, agent.transform.position, target);

			bool sample = NavMesh.SamplePosition(target, out var hit, 0.6f, new LayerMask() { value = 305624887 });

			DrawableLine.DrawableLines.ServerGenerateLine(Color.red, agent.transform.position, hit.position);

			if (sample)
			{
				//Logger.Debug($"Sample: {sample} {_agent.transform.position} {target} {hit.position} {hit.distance}");

				var path = new NavMeshPath();
				var pathCheck = agent.CalculatePath(target, path);
				//Logger.Debug($"job check (job location at {target}): {pathCheck} {path.status} {path.corners.Length} {hit.distance}");

				if (pathCheck)
				{
					//Logger.Debug($"has picked job at {target} (From {_agent.transform.position})");
					agent.ResetPath();
					agent.SetDestination(hit.position);
					agent.isStopped = false;

					DrawableLine.DrawableLines.ServerGenerateLine(Color.yellow, agent.transform.position, agent.destination);

					Logger.Debug($"New Destination: {sample} {hit.distance} {agent.gameObject.transform.position} {target} {agent.destination} {agent.remainingDistance} {agent.pathStatus} {agent.path.status} {agent.isOnNavMesh}");
				}
				else
				{
					Logger.Debug($"NPC {agent.name} PATHCHECKFAIL {target} {hit.distance}");
				}
			}
			else
			{
				Logger.Debug($"NPC {agent.name} Unable to get to location for job at {target} {hit.distance}");
			}
		}
	}
}
