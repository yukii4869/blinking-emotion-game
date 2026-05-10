using UnityEngine;

public class SmileSwarmEnemy : EnemyBase
{
    public Transform formationSlot;
    public SwarmRootMover swarmRoot;

    public override void UpdateBehavior()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        bool smiling = GameplayFaceInput.Instance != null &&
                       GameplayFaceInput.Instance.currentEmotion == Emotion.Happy;

        // Root steuern
        if (swarmRoot != null)
            swarmRoot.isWandering = smiling;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        // 1. Attack hat höchste Priorität
        if (!smiling && dist <= stats.attackRange)
        {
            SetState(EnemyState.Attack);
        }
        // 2. Chase wenn nicht lächeln
        else if (!smiling)
        {
            SetState(EnemyState.Chase);
        }
        // 3. Wander wenn lächeln
        else
        {
            SetState(EnemyState.Wander);
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
