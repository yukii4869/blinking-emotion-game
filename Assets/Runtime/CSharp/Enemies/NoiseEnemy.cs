using UnityEngine;

public class NoiseEnemy : EnemyBase, INoiseListener
{
    private Vector3 noiseTarget;
    private float investigateTimer = 0f;

    [Header("Noise Settings")]
    public float hearingRange = 12f;
    public float investigateDuration = 4f;

    private Vector3 wanderTarget;
    public float wanderRadius = 8f;
    public float wanderInterval = 5f;
    private float wanderTimer = 0f;

    public override void Start()
    {
        base.Start();
        NoiseManager.Register(this);
        PickNewWanderTarget();
    }

    private void OnDestroy()
    {
        NoiseManager.Unregister(this);
    }

    // Wird vom NoiseSystem aufgerufen
    public void OnNoiseHeard(Vector3 pos, float loudness)
    {
        float dist = Vector3.Distance(transform.position, pos);
        if (dist > hearingRange) return;

        noiseTarget = pos;
        investigateTimer = investigateDuration;

        SetState(EnemyState.Special); // Investigate
    }

    // WANDER
    protected override void UpdateIdle()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
            PickNewWanderTarget();

        agent.stoppingDistance = 0f;
        agent.SetDestination(wanderTarget);

        // Wenn Spieler in Range → Chase
        if (Vector3.Distance(transform.position, player.transform.position) < stats.attackRange * 2f)
            SetState(EnemyState.Chase);
    }

    private void PickNewWanderTarget()
    {
        wanderTimer = wanderInterval;

        Vector2 random = Random.insideUnitCircle * wanderRadius;
        wanderTarget = transform.position + new Vector3(random.x, 0, random.y);
    }

    // INVESTIGATE
    protected override void UpdateSpecial()
    {
        agent.stoppingDistance = 0f;
        agent.SetDestination(noiseTarget);

        investigateTimer -= Time.deltaTime;

        // Spieler in Range → Chase
        if (Vector3.Distance(transform.position, player.transform.position) < stats.attackRange * 2f)
        {
            SetState(EnemyState.Attack);
            return;
        }

        // Zeit abgelaufen → zurück zu Wander
        if (investigateTimer <= 0f)
        {
            SetState(EnemyState.Idle);
        }
    }
}
