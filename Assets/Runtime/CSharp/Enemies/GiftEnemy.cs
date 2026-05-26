using UnityEngine;
using UnityEngine.AI;

public class GiftGuest : EmotionEnemyBase
{
    public enum GiftState
    {
        Approach,
        OfferGift,
        ExpectSurprise,
        ExpectJoy,
        GiveGift,
        Disappointed,
        Angry,
        Attack
    }

    [Header("Gift Settings")]
    [SerializeField] private float emotionTimeout = 3f;
    [SerializeField] private GameObject giftItemPrefab;

    private GiftState currentState;
    private float emotionTimer;

    public override void Start()
    {
        base.Start();
        SetState(GiftState.Approach);
    }

    public override void UpdateBehavior()
    {
        if (stunned) return;

        switch (currentState)
        {
            case GiftState.Approach:
                UpdateApproach();
                break;

            case GiftState.OfferGift:
                UpdateOfferGift();
                break;

            case GiftState.ExpectSurprise:
                UpdateExpectSurprise();
                break;

            case GiftState.ExpectJoy:
                UpdateExpectJoy();
                break;

            case GiftState.Disappointed:
                UpdateDisappointed();
                break;

            case GiftState.Angry:
                UpdateAngry();
                break;

            case GiftState.Attack:
                UpdateAttack();
                break;
        }
    }

    // ---------------------------------------------------------
    // STATE MACHINE
    // ---------------------------------------------------------

    private void SetState(GiftState newState)
    {
        currentState = newState;
        emotionTimer = emotionTimeout;

        switch (newState)
        {
            case GiftState.OfferGift:
                agent.ResetPath();
                break;

            case GiftState.GiveGift:
                GiveGiftToPlayer();
                break;

            case GiftState.Disappointed:
                agent.ResetPath();
                break;

            case GiftState.Angry:
                // später Animation
                break;

            case GiftState.Attack:
                // später Animation
                break;
        }
    }

    // ---------------------------------------------------------
    // STATE LOGIC
    // ---------------------------------------------------------

    private void UpdateApproach()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        agent.stoppingDistance = stats.stopDistance;
        agent.speed = stats.moveSpeed;
        agent.SetDestination(player.transform.position);

        if (dist <= stats.stopDistance + 0.5f)
        {
            SetState(GiftState.OfferGift);
        }
    }

    private void UpdateOfferGift()
    {
        LookAtPlayer();
        SetState(GiftState.ExpectSurprise);
    }

    private void UpdateExpectSurprise()
    {
        LookAtPlayer();
        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Surprised)
        {
            SetState(GiftState.ExpectJoy);
            return;
        }

        if (emotionTimer <= 0f)
        {
            SetState(GiftState.Disappointed);
        }
    }

    private void UpdateExpectJoy()
    {
        LookAtPlayer();
        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Happy)
        {
            SetState(GiftState.GiveGift);
            return;
        }

        if (emotionTimer <= 0f)
        {
            SetState(GiftState.Disappointed);
        }
    }

    private void UpdateDisappointed()
    {
        emotionTimer -= Time.deltaTime;

        if (emotionTimer <= 0f)
        {
            SetState(GiftState.Angry);
        }
    }

    private void UpdateAngry()
    {
        agent.SetDestination(player.transform.position);

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist <= stats.attackRange)
        {
            SetState(GiftState.Attack);
        }
    }

    private void UpdateAttack()
    {
        LookAtPlayer();
        AttackBehavior(); // EnemyBase Attack
    }

    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    private void GiveGiftToPlayer()
    {
        Instantiate(giftItemPrefab, transform.position + transform.forward, Quaternion.identity);
        Destroy(gameObject, 1f);
    }
}
