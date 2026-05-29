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
        Attack,
        Leave
    }

    [Header("Gift Settings")]
    [SerializeField] private float emotionTimeout = 3f;
    [SerializeField] private GameObject giftItemPrefab;
    [SerializeField] private GameObject eyebroesAngry;
    [SerializeField] private GameObject eyebrowsNormal;
    [SerializeField] private Animator animator;

    private Vector3 leaveTarget;

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
        Debug.Log($"[GiftGuest] Switching State: {currentState} → {newState}");

        currentState = newState;
        emotionTimer = emotionTimeout;

        switch (newState)
        {
            case GiftState.OfferGift:
                agent.ResetPath();
                animator.SetTrigger("OfferGift");
                break;

            case GiftState.GiveGift:
                Debug.Log("[GiftGuest] Giving gift to player!");
                GiveGiftToPlayer();
                break;

            case GiftState.Disappointed:
                Debug.Log("[GiftGuest] Player failed emotion check → Disappointed");
                agent.ResetPath();
                break;

            case GiftState.Angry:
                Debug.Log("[GiftGuest] Guest is now ANGRY!");
                break;

            case GiftState.Attack:
                Debug.Log("[GiftGuest] ATTACKING PLAYER!");
                break;
            case GiftState.Leave:
                UpdateLeave();
                break;
        }
    }

    // ---------------------------------------------------------
    // STATE LOGIC
    // ---------------------------------------------------------

    private void UpdateApproach()
    {
        Debug.Log("[GiftGuest] State: Approach");

        float dist = Vector3.Distance(transform.position, player.transform.position);

        agent.stoppingDistance = stats.stopDistance;
        agent.speed = stats.moveSpeed;
        agent.SetDestination(player.transform.position);

        if (dist <= stats.stopDistance + 1f)
        {
            SetState(GiftState.OfferGift);
        }
    }

    private void UpdateOfferGift()
    {
        Debug.Log("[GiftGuest] State: OfferGift");
        LookAtPlayer();
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            SetState(GiftState.ExpectSurprise);
        }
    }

    private void UpdateExpectSurprise()
    {
        Debug.Log("[GiftGuest] State: ExpectSurprise");
        LookAtPlayer();
        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Surprised)
        {
            Debug.Log("[GiftGuest] Surprise detected! Moving to ExpectJoy.");
            SetState(GiftState.ExpectJoy);
            return;
        }

        if (emotionTimer <= 0f)
        {
            Debug.Log("[GiftGuest] Surprise NOT detected → Disappointed");
            SetState(GiftState.Disappointed);
        }
    }

    private void UpdateExpectJoy()
    {
        Debug.Log("[GiftGuest] State: ExpectJoy");
        LookAtPlayer();
        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Happy)
        {
            Debug.Log("[GiftGuest] Joy detected! Giving gift.");
            SetState(GiftState.GiveGift);
            return;
        }

        if (emotionTimer <= 0f)
        {
            Debug.Log("[GiftGuest] Joy NOT detected → Disappointed");
            SetState(GiftState.Disappointed);
        }
    }

    private void UpdateDisappointed()
    {
        Debug.Log("[GiftGuest] State: Disappointed");
        emotionTimer -= Time.deltaTime;

        if (emotionTimer <= 0f)
        {
            Debug.Log("[GiftGuest] Disappointment turned into ANGER!");
            SetState(GiftState.Angry);
        }
    }

    private void UpdateAngry()
    {
        Debug.Log("[GiftGuest] State: Angry");
        eyebrowsNormal.SetActive(false);
        eyebroesAngry.SetActive(true);

        agent.SetDestination(player.transform.position);

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist <= stats.attackRange)
        {
            SetState(GiftState.Attack);
        }
    }

    private void UpdateAttack()
    {
        Debug.Log("[GiftGuest] State: Attack");
        LookAtPlayer();
        AttackBehavior();
    }
    private void UpdateLeave()
    {
        Debug.Log("[GiftGuest] State: Leave");

        agent.stoppingDistance = 0f;
        agent.speed = stats.moveSpeed * 0.8f; // etwas langsamer
        agent.SetDestination(leaveTarget);

        float dist = Vector3.Distance(transform.position, leaveTarget);

        if (dist < 0.3f)
        {
            Debug.Log("[GiftGuest] Left the area. Despawning.");
            Destroy(gameObject);
        }
    }


    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    private void GiveGiftToPlayer()
    {
        Debug.Log("[GiftGuest] Spawning gift item.");
        Instantiate(giftItemPrefab, transform.position + transform.forward, Quaternion.identity);

        leaveTarget = transform.position - transform.forward * 5f;
        SetState(GiftState.Leave);
    }
}
