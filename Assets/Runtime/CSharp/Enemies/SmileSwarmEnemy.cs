using UnityEngine;

public class SmileSwarmEnemy : EmotionEnemyBase
{
    private enum SwarmState
    {
        Wander,
        Chase,
        Attack
    }

    public Transform formationSlot;
    public SwarmRootMover swarmRoot;

    private SwarmState swarmState = SwarmState.Wander;

    public override void Start()
    {
        base.Start();
        HandleEmotion(GameplayFaceInput.Instance.currentEmotion);
        SetState(EnemyState.Special); // läuft komplett über Special
    }

    protected override void HandleEmotion(Emotion e)
    {
        base.HandleEmotion(e);

        bool smiling = (e == Emotion.Happy);

        if (swarmRoot != null)
            swarmRoot.isWandering = smiling;

        swarmState = smiling ? SwarmState.Wander : SwarmState.Chase;
    }

    protected override void UpdateSpecial()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (swarmState == SwarmState.Chase && dist <= stats.attackRange)
            swarmState = SwarmState.Attack;

        switch (swarmState)
        {
            case SwarmState.Wander:
                DoFormationWander();
                break;

            case SwarmState.Chase:
                ApproachPlayer();
                break;

            case SwarmState.Attack:
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
