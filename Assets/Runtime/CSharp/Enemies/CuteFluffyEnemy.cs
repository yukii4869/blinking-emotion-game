using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CuteFluffyEnemy : EnemyBase
{
    private enum FluffyState
    {
        Approach,
        Flee,
    }
    [SerializeField] private Transform model;
    [SerializeField] private Animator animator;

    [Header("Flee Settings")]
    [SerializeField] private float fleeDistance = 5.0f;
    [SerializeField] private float fleeSpeed = 4.0f;
    [SerializeField] private float noiseLoudness = 1.0f;



    private FluffyState currentState = FluffyState.Approach;
    private float calmTimer = 0f;

    public override void TickBehavior()
    {
        bool angry = FaceInputManager.Instance.currentEmotion == Emotion.Angry;

        switch (currentState)
        {
            case FluffyState.Approach:
                TickApproach(angry);
                Debug.Log("Approach");
                break;

            case FluffyState.Flee:
                Flee(angry);
                Debug.Log("Flee");
                break;
        }
    }

    // -----------------------------
    // APPROACH
    // -----------------------------
    private void TickApproach(bool angry)
    {
        if (angry)
        {
            currentState = FluffyState.Flee;
            animator.SetBool("Bounce", false);
            return;
        }

        agent.speed = stats.moveSpeed;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist > stats.stopDistance)
        {
            agent.SetDestination(player.transform.position);
            animator.SetBool("Bounce", false);
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("Bounce", true);
            animator.SetInteger("BounceType", Random.Range(0, 3));
        }
    }


    // -----------------------------
    // FLEE
    // -----------------------------
    private void Flee(bool angry)
    {
        if (!angry)
        {
            StartCoroutine(Stun());
            currentState = FluffyState.Approach;
            agent.ResetPath();
            return;
        }
        animator.SetBool("Bounce", false);
        agent.speed = fleeSpeed;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist >= fleeDistance)
        {
            agent.ResetPath();
            return; // <-- WICHTIG
        }

        Vector3 dir = (transform.position - player.transform.position).normalized;

        if (dir.sqrMagnitude < 0.1f)
            dir = -player.transform.forward;

        Vector3 fleeTarget = transform.position + dir * fleeDistance;

        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, 6f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(transform.position + dir * 3f);
    }
}
