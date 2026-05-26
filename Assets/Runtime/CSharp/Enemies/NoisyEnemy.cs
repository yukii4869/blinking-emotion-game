using UnityEngine;

public class NoiseEnemy : EnemyBase
{
    [Header("Noise Settings")]
    public float noiseThreshold = 0.02f;
    public float hearingRange = 8f;
    public float alertDuration = 1.5f;

    private float smoothedLoudness;
    private float smoothSpeed = 10f;
    private float alertTimer;
    private WanderComponent wander;

    public override void Start()
    {
        base.Start();
        wander = GetComponent<WanderComponent>();
    }

    public override void UpdateBehavior()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            agent.isStopped = true;
            Debug.Log($"[NoiseEnemy] Game not in Gameplay → STOPPED");
            return;
        }

        agent.isStopped = false;

        // Lautstärke holen
        float loudness = MicInputManager.Instance.GetLoudness();
        smoothedLoudness = Mathf.Lerp(smoothedLoudness, loudness, Time.deltaTime * smoothSpeed);

        float dist = Vector3.Distance(transform.position, player.transform.position);
        bool heardNoise = smoothedLoudness > noiseThreshold;

        // --- DEBUG ---
        Debug.Log(
            $"[NoiseEnemy] Loudness(raw={loudness:F4}, smooth={smoothedLoudness:F4}), " +
            $"Dist={dist:F2}, HeardNoise={heardNoise}"
        );

        // --- STATE PRIORITÄTEN ---
        EnemyState newState = CurrentState;

        // 1. Attack
        if (dist <= stats.attackRange)
        {
            newState = EnemyState.Attack;
        }
        // 2. Chase wenn Geräusch + in Hörreichweite
        else if (heardNoise && dist <= hearingRange)
        {
            newState = EnemyState.Chase;
        }
        // 3. Alert wenn Geräusch aber zu weit weg
        else if (heardNoise)
        {
            newState = EnemyState.Alert;
            alertTimer = alertDuration;
        }
        // 4. Sonst wandern
        else
        {
            newState = EnemyState.Wander;
        }

        // --- DEBUG: State-Wechsel ---
        if (newState != CurrentState)
        {
            Debug.Log($"[NoiseEnemy] STATE CHANGE: {CurrentState} → {newState}");
            SetState(newState);
        }

        // --- STATE AUSFÜHREN ---
        switch (CurrentState)
        {
            case EnemyState.Wander:
                Debug.Log("[NoiseEnemy] Executing WanderBehavior()");
                WanderBehavior();
                break;

            case EnemyState.Alert:
                Debug.Log("[NoiseEnemy] Executing AlertBehavior()");
                AlertBehavior();
                break;

            case EnemyState.Chase:
                Debug.Log("[NoiseEnemy] Executing ChaseBehavior()");
                ChaseBehavior();
                break;

            case EnemyState.Attack:
                Debug.Log("[NoiseEnemy] Executing AttackBehavior()");
                AttackBehavior();
                break;
        }
    }

    private void WanderBehavior()
    {
        Debug.Log("[NoiseEnemy] Wander → moving randomly");
       wander.Tick();
    }

    private void AlertBehavior()
    {
        alertTimer -= Time.deltaTime;

        Debug.Log($"[NoiseEnemy] Alert → Looking at player, timer={alertTimer:F2}");

        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        if (alertTimer <= 0)
        {
            Debug.Log("[NoiseEnemy] Alert finished → back to Wander");
            SetState(EnemyState.Wander);
        }
    }

    protected override void ChaseBehavior()
    {
        Debug.Log("[NoiseEnemy] Chase → Moving toward player");
        agent.stoppingDistance = stats.stopDistance;
        agent.SetDestination(player.transform.position);
    }

    protected override void AttackBehavior()
    {
        Debug.Log("[NoiseEnemy] ATTACK → Player in range!");
        base.AttackBehavior();
    }
}
