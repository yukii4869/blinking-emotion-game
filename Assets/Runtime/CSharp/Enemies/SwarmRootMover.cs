using UnityEngine;
using UnityEngine.AI;

public class SwarmRootMover : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool isWandering = false;

    public float wanderRadius = 6f;
    public float sampleRadius = 2f;

    void Update()
    {
        if (!isWandering)
        {
            agent.ResetPath();
            return;
        }

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir.y = 0;

            if (NavMesh.SamplePosition(transform.position + randomDir, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }
}
