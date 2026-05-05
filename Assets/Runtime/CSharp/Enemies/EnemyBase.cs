using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public EnemyStats stats;
    protected NavMeshAgent agent;
    protected PlayerController player;
    protected bool stunned = false;
    private float lastAttackTime;
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<PlayerController>();
    }
    protected virtual void Update()
    {
        /// Warten bis Systeme bereit sind
        if (GameplayFaceInput.Instance == null ||
            PlayerVision.Instance == null ||
            MediaPipeProvider.Instance == null)
            return;

        UpdateBehavior();

    }

    // Jede Gegnerart implementiert ihre eigene Logik
    public virtual void UpdateBehavior()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Pause)
        {
            agent.isStopped = true;
            return;
        }

        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
    }


    protected virtual void TryAttack()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        // Zu weit weg → kein Angriff
        if (dist > stats.attackRange)
            return;

        // Cooldown
        if (Time.time < lastAttackTime + stats.attackCooldown)
            return;

        lastAttackTime = Time.time;

        // Schaden
        //player.TakeDamage(stats.damage);

        // Knockback
        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = stats.knockbackUpwardForce;

        player.ApplyKnockback(dir, stats.knockbackForce, stats.knockbackUpwardForce);
    }
    protected IEnumerator Stun()
    {
        stunned = true;
        yield return new WaitForSeconds(stats.stunDuration);
        stunned = false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (stats == null)
            return;

        // Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);

        // Stop Distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.stopDistance);
    }
}
