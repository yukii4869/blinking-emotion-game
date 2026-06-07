using UnityEngine;

public class NoiseEnemy : EnemyBase, INoiseListener
{
    private Vector3 lastHeardNoise;
    private float investigateTimer = 0f;

    public float investigateDuration = 3f;
    public float hearingRange = 12f;

    public override void Start()
    {
        base.Start();
        NoiseManager.Register(this);
    }

    private void OnDestroy()
    {
        NoiseManager.Unregister(this);
    }

    public void OnNoiseHeard(Vector3 pos, float loudness)
    {
        float dist = Vector3.Distance(transform.position, pos);
        if (dist > hearingRange) return;

        lastHeardNoise = pos;
        investigateTimer = investigateDuration;

        SetState(EnemyState.Special); // Special = Investigate
    }

    protected override void UpdateSpecial()
    {
        // Investigate
        agent.stoppingDistance = 0f;
        agent.SetDestination(lastHeardNoise);

        investigateTimer -= Time.deltaTime;

        // Wenn Spieler in der Nähe → Chase
        if (Vector3.Distance(transform.position, player.transform.position) < stats.attackRange * 2f)
        {
            SetState(EnemyState.Chase);
            return;
        }

        // Wenn Zeit abgelaufen → Idle
        if (investigateTimer <= 0f)
        {
            SetState(EnemyState.Idle);
        }
    }
}
