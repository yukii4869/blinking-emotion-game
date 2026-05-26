using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    public EnemyStats stats;

    protected NavMeshAgent agent;
    protected PlayerController player;

    protected bool stunned = false;
    protected float lastAttackTime = 0f;

    public EnemyState CurrentState { get; private set; }

    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<PlayerController>();
        agent.speed = stats.moveSpeed;
    }

    protected virtual void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        UpdateBehavior();
    }

    public void SetState(EnemyState newState)
    {
        CurrentState = newState;
    }

    public abstract void UpdateBehavior();

    protected virtual void AttackBehavior()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist > stats.attackRange) return;

        if (Time.time < lastAttackTime + stats.attackCooldown) return;
        lastAttackTime = Time.time;

        PlayerHealth.Instance.TakeDamage(stats.damage);

        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = stats.knockbackUpwardForce;
        player.ApplyKnockback(dir, stats.knockbackForce, stats.knockbackUpwardForce);
    }
    protected virtual void ChaseBehavior()
    {
        if (player == null) return;

        agent.stoppingDistance = stats.stopDistance;
        agent.speed = stats.moveSpeed;
        agent.SetDestination(player.transform.position);
    }

    protected IEnumerator Stun()
    {
        stunned = true;
        yield return new WaitForSeconds(stats.stunDuration);
        stunned = false;
    }
    protected void LookAtPlayer(float rotationSpeed = 5f)
    {
        if (player == null) return;

        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

}
