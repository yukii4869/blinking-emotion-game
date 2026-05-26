using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WanderComponent : MonoBehaviour
{
    public float wanderRadius = 8f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public float lookSpeed = 120f;

    private NavMeshAgent agent;
    private bool isLookingAround = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
    }

    public void Tick()
    {
        if (isLookingAround) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(LookAroundRoutine());
        }
    }

    private IEnumerator LookAroundRoutine()
    {
        isLookingAround = true;

        float idleTime = Random.Range(minIdleTime, maxIdleTime);
        float timer = idleTime;

        Quaternion startRot = transform.rotation;
        float targetAngle = Random.Range(-60f, 60f);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            Quaternion targetRot = Quaternion.Euler(
                0,
                startRot.eulerAngles.y + targetAngle,
                0
            );

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                lookSpeed * Time.deltaTime
            );

            yield return null;
        }

        SetNewDestination();
        isLookingAround = false;
    }

    private void SetNewDestination()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMesh.SamplePosition(randomPos, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }
}
