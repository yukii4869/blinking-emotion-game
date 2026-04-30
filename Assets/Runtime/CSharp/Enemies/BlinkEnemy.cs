using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BlinkEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float stepDistance = 0.3f;
    [SerializeField] private float moveCooldown = 0.5f;
    [SerializeField] private float personalSpace = 1.2f;
    private float lastMoveTime = 0f;

    [Header("Combat")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 2f;
    private bool stunned = false;

    [Header("References")]
    private NavMeshAgent agent;
    private PlayerController player;

    public override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        player = FindFirstObjectByType<PlayerController>();
    }

    public override void TickBehavior()
{
    float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
    bool blinking = FaceInputManager.Instance.isBlinking;
    bool looking = PlayerVision.Instance.IsLookingAt(transform);

    // 1) Stun blockiert alles
    if (stunned)
    {
        agent.ResetPath();
        return;
    }

    // 2) Angriff: nur wenn nah UND (wegschauen ODER blinzeln)
    if (dist < attackDistance && (!looking || blinking))
    {
        agent.ResetPath();
        TryAttack();
        return;
    }

    // 3) Stoppen: wenn angeschaut UND nicht blinzeln
    if (looking && !blinking)
    {
        agent.ResetPath();
        return;
    }

    // 4) Bewegung: wenn wegschauen ODER blinzeln
    Move();
}


    private void Move()
    {
        if (Time.time < lastMoveTime + moveCooldown)
            return;

        agent.SetDestination(Camera.main.transform.position);

        if (agent.path.corners.Length < 2)
            return;

        Vector3 nextCorner = agent.path.corners[1];
        Vector3 dir = (nextCorner - transform.position).normalized;

        transform.position += dir * stepDistance;
        agent.nextPosition = transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        lastMoveTime = Time.time;
    }

    private void TryAttack()
    {
        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (PlayerVision.Instance.IsLookingAt(transform))
            return;

        if (dist < attackDistance && Time.time > lastAttackTime + attackCooldown)
        {
            Vector3 dir = player.transform.position - transform.position;
            player.ApplyKnockback(dir);

            lastAttackTime = Time.time;
            StartCoroutine(Stun());
        }
    }

    private IEnumerator Stun()
    {
        stunned = true;
        yield return new WaitForSeconds(stunDuration);
        stunned = false;
    }
}
