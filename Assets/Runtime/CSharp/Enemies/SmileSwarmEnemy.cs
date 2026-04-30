using UnityEngine;
using UnityEngine.AI;

public class SmileSwarmEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float attackDistance = 1.2f;

    [Header("Attack Settings")]
    [SerializeField] private float knockbackStrength = 8f;
    [SerializeField] private float knockbackUpward = 2f;
    [SerializeField] private int damage = 1;

    private NavMeshAgent agent;
    private PlayerController player;

    public override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<PlayerController>();
    }

    public override void TickBehavior()
    {
        bool smiling = FaceInputManager.Instance.currentEmotion == Emotion.Happy;
        float dist = Vector3.Distance(transform.position, player.transform.position);

        // 1) Spieler lächelt → Idle
        if (smiling)
        {
            agent.ResetPath();
            return;
        }

        // 2) Spieler lächelt NICHT → Aggro
        if (dist > stopDistance)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.ResetPath();
        }

        // 3) Attack
        if (dist < attackDistance)
        {
            Vector3 dir = player.transform.position - transform.position;
            player.ApplyKnockback(dir, knockbackStrength, knockbackUpward);
        }
    }
}
