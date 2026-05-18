using UnityEngine;

public class SmileSwarmEnemy : EmotionEnemyBase
{
    public Transform formationSlot;
    public SwarmRootMover swarmRoot;

    // Emotion → State Mapping:
    // Happy  → Wander
    // Not Happy → Chase/Attack

    protected override void HandleEmotion(Emotion e)
    {
        base.HandleEmotion(e);

        bool smiling = (e == Emotion.Happy);

        // Root Movement steuern
        if (swarmRoot != null)
            swarmRoot.isWandering = smiling;

        // Statewechsel basierend auf Emotion
        if (smiling)
        {
            SetState(EnemyState.Wander);
        }
        else
        {
            // Wenn nicht lächelnd → Chase (Attack wird später im Update geprüft)
            SetState(EnemyState.Chase);
        }
    }

    public override void UpdateBehavior()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        // Attack hat höchste Priorität
        if (currentEmotion != Emotion.Happy && dist <= stats.attackRange)
        {
            SetState(EnemyState.Attack);
        }

        // State ausführen
        switch (CurrentState)
        {
            case EnemyState.Wander:
                WanderBehavior();
                break;

            case EnemyState.Chase:
                ChaseBehavior();
                break;

            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
    }

    protected override void WanderBehavior()
    {
        if (formationSlot == null) return;

        agent.stoppingDistance = 0.1f;
        agent.SetDestination(formationSlot.position);
    }

    protected override void ChaseBehavior()
    {
        if (player == null) return;

        agent.stoppingDistance = stats.stopDistance;
        agent.SetDestination(player.transform.position);
    }
}
