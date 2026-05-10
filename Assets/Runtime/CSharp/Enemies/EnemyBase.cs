using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public EnemyStats stats;

    [Header("Wander System")]
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;
    [SerializeField] private float lookSpeed = 120f;
    public EnemyState CurrentState { get; private set; }
    protected NavMeshAgent agent;
    protected PlayerController player;
    protected bool stunned = false;
    private float lastAttackTime;


    private bool isLookingAround = false;
    private float lookTimer;
    private Quaternion startRot;
    private float targetAngle;

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
    public virtual void SetState(EnemyState newState)
    {
        CurrentState = newState;
    }

    // Jede Gegnerart implementiert ihre eigene Logik
    public virtual void UpdateBehavior()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Pause ||
            GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        switch (CurrentState)
        {
            case EnemyState.Wander:
                WanderBehavior();
                break;

            case EnemyState.Alert:
                AlertBehavior();
                break;

            case EnemyState.Chase:
                ChaseBehavior();
                break;

            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
    }
    protected virtual void WanderBehavior()
    {
        if (isLookingAround)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(LookAroundRoutine());
        }
    }

    protected virtual void AlertBehavior() { }
    protected virtual void ChaseBehavior() { }
    protected virtual void AttackBehavior()
    {
         agent.ResetPath();
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
    // Wander Hilfs-Funktionen
    private IEnumerator LookAroundRoutine()
    {
        isLookingAround = true;

        float idleTime = Random.Range(minIdleTime, maxIdleTime);
        lookTimer = idleTime;

        startRot = transform.rotation;
        targetAngle = Random.Range(-60f, 60f);

        while (lookTimer > 0)
        {
            lookTimer -= Time.deltaTime;

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

        SetNewWanderDestination();
        isLookingAround = false;
    }

    private void SetNewWanderDestination()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMesh.SamplePosition(randomPos, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
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
