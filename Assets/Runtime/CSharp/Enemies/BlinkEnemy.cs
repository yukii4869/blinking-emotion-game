using UnityEngine;
using UnityEngine.AI;

public class BlinkEnemy : EnemyBase
{
    [Header("Blink Movement")]
    [SerializeField] private float moveCooldown = 0.4f;
    [SerializeField] private float stepSize = 0.8f;

    private float lastMoveTime = 0f;
    private bool blinked = false;
    private float blinkTimer = 0f;
    private float blinkDuration = 0.2f;

    public override void Start()
    {
        base.Start();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        GameplayFaceInput.OnBlink += OnBlink;
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnBlink -= OnBlink;
    }

    private void OnBlink()
    {
        blinked = true;
        blinkTimer = blinkDuration;
    }

    public override void UpdateBehavior()
    {
        // Blink decay
        if (blinkTimer > 0f)
            blinkTimer -= Time.deltaTime;
        else
            blinked = false;

        if (stunned)
        {
            agent.ResetPath();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.transform.position);
        bool looking = PlayerVision.Instance.IsInView(transform);

        //  ATTACK-BEDINGUNG (final):
        // Wenn in AttackRange UND (nicht schauen ODER blinzeln)
        if (dist <= stats.attackRange && (!looking || blinked))
        {
            AttackBehavior();
            StartCoroutine(Stun());
            return;
        }

        //  STATUE: Spieler schaut hin & blinzelt NICHT
        if (looking && !blinked)
        {
            agent.ResetPath();
            LookAtPlayer();
            return;
        }

        //  STOPDISTANCE: Nur Anti-Clipping, NICHT Attack-Blocker
        if (dist < stats.stopDistance && dist > stats.attackRange)
        {
            agent.ResetPath();
            LookAtPlayer();
            return;
        }

        // CHASE: Spieler schaut weg ODER blinzelt
        ApproachPlayer();
    }

    protected override void ApproachPlayer()
    {
        if (Time.time < lastMoveTime + moveCooldown)
            return;

        agent.SetDestination(player.transform.position);

        if (agent.path.corners.Length < 2)
            return;

        Vector3 nextCorner = agent.path.corners[1];
        Vector3 dir = (nextCorner - transform.position).normalized;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        float step = Mathf.Min(stepSize, dist - stats.stopDistance);

        if (step > 0f)
        {
            transform.position += dir * step;
            agent.nextPosition = transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        lastMoveTime = Time.time;
    }
}
