using UnityEngine;
using UnityEngine.AI;

public class SmileSwarmEnemy : EnemyBase
{

    public override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerController>();
    }

    public override void UpdateBehavior()
    {
        base.UpdateBehavior();
        bool smiling = GameplayFaceInput.Instance.currentEmotion == Emotion.Happy;
        float dist = Vector3.Distance(transform.position, player.transform.position);

        // 1) Spieler lächelt → Idle
        if (smiling)
        {
            agent.ResetPath();
            return;
        }

        // 2) Spieler lächelt NICHT → Aggro
        if (dist > stats.stopDistance)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.ResetPath();
        }

        // 3) Attack
        if (dist < stats.attackRange)
        {
           TryAttack();
        }
    }
}
