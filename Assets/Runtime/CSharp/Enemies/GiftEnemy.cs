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
    [SerializeField] private GameObject lookAngry;
    [SerializeField] private GameObject lookExpectSmile;
    [SerializeField] private GameObject lookSurprised;
    [SerializeField] private GameObject lookDisappointed;
    [SerializeField] private Animator armAnimator;
    [SerializeField] private GameObject handGiftObject;

    private GiftState currentState;
    private float emotionTimer;
    private bool missedSurprise;

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
            case GiftState.Approach: UpdateApproach(); break;
            case GiftState.OfferGift: UpdateOfferGift(); break;
            case GiftState.ExpectSurprise: UpdateExpectSurprise(); break;
            case GiftState.ExpectJoy: UpdateExpectJoy(); break;
            case GiftState.GiveGift: UpdateGiveGift(); break;
            case GiftState.Disappointed: UpdateDisappointed(); break;
            case GiftState.Angry: UpdateAngry(); break;
            case GiftState.Attack: UpdateAttack(); break;
            case GiftState.Leave: UpdateLeave(); break;

        }
    }

    // ---------------------------------------------------------
    // STATE MACHINE
    // ---------------------------------------------------------

    private void SetState(GiftState newState)
    {
        currentState = newState;
        emotionTimer = emotionTimeout;
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

        if (dist <= stats.stopDistance + 1f)
            SetState(GiftState.OfferGift);
    }

    private void UpdateOfferGift()
    {
        LookAtPlayer();
        SetFace(FaceType.Surprised);

        armAnimator.SetTrigger("OfferGift");

        if (armAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            SetState(GiftState.ExpectSurprise);
    }

    private void UpdateExpectSurprise()
    {
        LookAtPlayer();
        SetFace(FaceType.Surprised);

        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Surprised)
        {
            missedSurprise = false;
            SetState(GiftState.ExpectJoy);
            return;
        }

        if (emotionTimer <= 0f)
        {
            missedSurprise = true;
            SetState(GiftState.Disappointed);
        }
    }

    private void UpdateExpectJoy()
    {
        LookAtPlayer();
        SetFace(FaceType.ExpectSmile);

        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Happy)
        {
            SetState(GiftState.GiveGift);
            return;
        }

        if (emotionTimer <= 0f)
            SetState(GiftState.Disappointed);
    }

    private void UpdateGiveGift()
    {
        GiveGiftToPlayer();
        SetState(GiftState.Leave);
    }
    private void UpdateLeave()
    {
        GoToRoom();
    }

    private void UpdateDisappointed()
    {
        LookAtPlayer();
        SetFace(FaceType.Disappointed);

        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Surprised)
        {
            SetState(GiftState.ExpectJoy);
            return;
        }

        if (currentEmotion == Emotion.Happy)
        {
            if (missedSurprise)
                SetState(GiftState.ExpectSurprise);
            else
                SetState(GiftState.GiveGift);

            return;
        }

        if (emotionTimer <= 0f)
            SetState(GiftState.Angry);
    }

    private void UpdateAngry()
    {
        SetFace(FaceType.Angry);
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) <= stats.attackRange)
            SetState(GiftState.Attack);
    }

    private void UpdateAttack()
    {
        LookAtPlayer();
        AttackBehavior();
        SetState(GiftState.Leave);
    }

    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    private void GiveGiftToPlayer()
    {
        if (handGiftObject != null)
            handGiftObject.SetActive(false);

        Instantiate(giftItemPrefab, transform.position + transform.forward, Quaternion.identity);
    }

    private void SetFace(FaceType face)
    {
        lookAngry.SetActive(false);
        lookExpectSmile.SetActive(false);
        lookSurprised.SetActive(false);
        lookDisappointed.SetActive(false);

        switch (face)
        {
            case FaceType.Surprised: lookSurprised.SetActive(true); break;
            case FaceType.Disappointed: lookDisappointed.SetActive(true); break;
            case FaceType.ExpectSmile: lookExpectSmile.SetActive(true); break;
            case FaceType.Angry: lookAngry.SetActive(true); break;
        }
    }

    public enum FaceType
    {
        Angry,
        ExpectSmile,
        Surprised,
        Disappointed
    }
}
