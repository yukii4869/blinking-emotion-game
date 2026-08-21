using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CuteFluffyEnemy : EmotionEnemyBase
{
    private enum FluffyState
    {
        Wander,
        Approach,
        Flee,
    }

    [SerializeField] private Transform model;
    [SerializeField] private Animator animator;

    [Header("Flee Settings")]
    [SerializeField] private float fleeDistance = 5.0f;
    [SerializeField] private float fleeSpeed = 4.0f;
    private Vector3 fleeTarget;
    private bool hasFleeTarget = false;
    private FluffyState currentState = FluffyState.Approach;
    private float lifeTimer = 0f;

    public override void Start()
    {
        base.Start();
        agent.updateRotation = false;
        SetState(EnemyState.Special);
        currentState = FluffyState.Approach;
    }

    // -------------------------
    // EMOTION EVENT HANDLING
    // -------------------------
    protected override void HandleEmotion(Emotion e)
    {
        base.HandleEmotion(e);

        if (e == Emotion.Angry)
        {
            currentState = FluffyState.Flee;
            animator.SetBool("Bounce", false);
        }
        else
        {
            // Spieler ist nicht mehr wütend → zurück zu Approach
            if (currentState == FluffyState.Flee)
            {
                StartCoroutine(Stun());
                currentState = FluffyState.Approach;
                agent.ResetPath();
            }
        }
    }

    // -------------------------
    // ROTATION / LOOK AT PLAYER
    // -------------------------
    private void LateUpdate()
    {
        Vector3 desiredDir;

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            desiredDir = agent.velocity.normalized;
        }
        else
        {
            desiredDir = (player.transform.position - transform.position).normalized;
        }

        desiredDir.y = 0;

        if (desiredDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            model.rotation = Quaternion.Slerp(model.rotation, targetRot, Time.deltaTime * 8f);
        }
    }

    // -------------------------
    // MAIN BEHAVIOR LOOP
    // -------------------------
    protected override void UpdateSpecial()
    {
        // DESPAWN: Timer
        lifeTimer += Time.deltaTime;
        if (lifeTimer > 120f)
        {
            SetState(EnemyState.GoingHome);
            return;
        }

        switch (currentState)
        {
            case FluffyState.Wander:
                break;

            case FluffyState.Approach:
                Approach();
                break;

            case FluffyState.Flee:
                Flee();
                break;
        }
    }


    // -------------------------
    // APPROACH PLAYER
    // -------------------------
    private void Approach()
    {
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

    // -------------------------
    // FLEE FROM PLAYER
    // -------------------------
    private void Flee()
    {
        animator.SetBool("Bounce", false);
        agent.speed = fleeSpeed;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        // Wenn weit genug weg → stoppen
        if (dist >= fleeDistance)
        {
            agent.ResetPath();
            hasFleeTarget = false;
            return;
        }

        // Nur EINMAL ein Flee-Ziel setzen
        if (!hasFleeTarget)
        {
            Vector3 dir = (transform.position - player.transform.position).normalized;

            if (dir.sqrMagnitude < 0.1f)
                dir = -player.transform.forward;

            fleeTarget = transform.position + dir * fleeDistance;

            // NavMesh validieren
            if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, 6f, NavMesh.AllAreas))
                fleeTarget = hit.position;

            agent.SetDestination(fleeTarget);
            hasFleeTarget = true;
        }

        // Wenn Ziel erreicht → neues Ziel setzen
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            hasFleeTarget = false;
    }
}
