using UnityEngine;
using UnityEngine.AI;

public class BlinkEnemy : EnemyBase
{
    [Header("Blink Movement")]
    [SerializeField] private float moveCooldown = 0.4f;
    [SerializeField] private float stepSize = 0.8f;
    private float lifeTimer = 0f;

    private float lastMoveTime = 0f;
    private bool blinked = false;
    private float blinkTimer = 0f;
    private float blinkDuration = 0.2f;

    public override void Start()
    {
        base.Start();
        agent.updatePosition = false;
        agent.updateRotation = false;
        SetState(EnemyState.Special);
    }

    private void OnEnable()
    {
        InputSelector.Instance.ActiveInput.OnBlink += OnBlink;
    }

    private void OnDisable()
    {
        InputSelector.Instance.ActiveInput.OnBlink -= OnBlink;
    }

    private void OnBlink()
    {
        blinked = true;
        blinkTimer = blinkDuration;
    }


    private bool HasLineOfSight()
    {
        Vector3 eye = player.transform.position + Vector3.up * 1.6f;
        Vector3 dir = (transform.position - eye).normalized;
        float dist = Vector3.Distance(eye, transform.position);

        if (Physics.Raycast(eye, dir, out RaycastHit hit, dist))
        {
            return hit.transform == transform;
        }

        return false;
    }
    protected override void UpdateSpecial()
    {
        // DESPAWN: Timer
        lifeTimer += Time.deltaTime;
        if (lifeTimer > 180f)
        {
            Destroy(gameObject);
            return;
        }
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
        bool looking = PlayerVision.Instance.IsInView(transform) && HasLineOfSight();

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
        ApproachStepByStep();
    }

    private void ApproachStepByStep()
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
