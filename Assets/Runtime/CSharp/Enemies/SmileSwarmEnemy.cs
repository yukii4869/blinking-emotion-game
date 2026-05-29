using UnityEngine;

public class SmileSwarmEnemy : EmotionEnemyBase
{
    public Transform formationSlot;
    public SwarmRootMover swarmRoot;

    public override void Start()
    {
        base.Start();
        HandleEmotion(GameplayFaceInput.Instance.currentEmotion);
    }

    protected override void HandleEmotion(Emotion e)
    {
        bool smiling = (e == Emotion.Happy);

        if (swarmRoot != null)
            swarmRoot.isWandering = smiling;

        SetState(smiling ? EnemyState.Wander : EnemyState.Chase);
    }

    public override void UpdateBehavior()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (CurrentState == EnemyState.Chase && dist <= stats.attackRange)
            SetState(EnemyState.Attack);

        switch (CurrentState)
        {
            case EnemyState.Wander:
                DoFormationWander();
                break;

            case EnemyState.Chase:
                ApproachPlayer();
                break;

            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
    }

    private void DoFormationWander()
    {
        if (formationSlot == null) return;
        agent.stoppingDistance = 0.1f;
        agent.SetDestination(formationSlot.position);
    }
}
