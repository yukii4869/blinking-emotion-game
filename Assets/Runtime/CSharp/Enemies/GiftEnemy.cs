using UnityEngine;

public class GiftGuest : EmotionEnemyBase
{
    private enum GiftState
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

    private GiftState giftState;
    private float emotionTimer;
    private bool missedSurprise;

    public override void Start()
    {
        base.Start();

        giftState = GiftState.Approach;
        SetState(EnemyState.Special); // GiftGuest läuft komplett über Special
    }

    // ---------------------------------------------------------
    // SPECIAL STATE MACHINE
    // ---------------------------------------------------------
    protected override void UpdateSpecial()
    {
        switch (giftState)
        {
            case GiftState.Approach:         UpdateApproachGift(); break;
            case GiftState.OfferGift:        UpdateOfferGift(); break;
            case GiftState.ExpectSurprise:   UpdateExpectSurprise(); break;
            case GiftState.ExpectJoy:        UpdateExpectJoy(); break;
            case GiftState.GiveGift:         UpdateGiveGift(); break;
            case GiftState.Disappointed:     UpdateDisappointed(); break;
            case GiftState.Angry:            UpdateAngry(); break;
            case GiftState.Attack:           UpdateAttackGift(); break;
            case GiftState.Leave:            UpdateLeave(); break;
        }
    }

    private void SetGiftState(GiftState newState)
    {
        giftState = newState;
        emotionTimer = emotionTimeout;
    }

    // ---------------------------------------------------------
    // STATE LOGIC
    // ---------------------------------------------------------

    private void UpdateApproachGift()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        ApproachPlayer(); // EnemyBase Movement

        if (dist <= stats.stopDistance + 1f)
            SetGiftState(GiftState.OfferGift);
    }

    private void UpdateOfferGift()
    {
        LookAtPlayer();
        SetFace(FaceType.Surprised);

        armAnimator.SetTrigger("OfferGift");

        if (armAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            SetGiftState(GiftState.ExpectSurprise);
    }

    private void UpdateExpectSurprise()
    {
        LookAtPlayer();
        SetFace(FaceType.Surprised);

        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Surprised)
        {
            missedSurprise = false;
            SetGiftState(GiftState.ExpectJoy);
            return;
        }

        if (emotionTimer <= 0f)
        {
            missedSurprise = true;
            SetGiftState(GiftState.Disappointed);
        }
    }

    private void UpdateExpectJoy()
    {
        LookAtPlayer();
        SetFace(FaceType.ExpectSmile);

        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Happy)
        {
            SetGiftState(GiftState.GiveGift);
            return;
        }

        if (emotionTimer <= 0f)
            SetGiftState(GiftState.Disappointed);
    }

    private void UpdateGiveGift()
    {
        GiveGiftToPlayer();
        SetGiftState(GiftState.Leave);
    }

    private void UpdateLeave()
    {
        GoToRoom(); // EnemyBase Heimweg
    }

    private void UpdateDisappointed()
    {
        LookAtPlayer();
        SetFace(FaceType.Disappointed);

        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Surprised)
        {
            SetGiftState(GiftState.ExpectJoy);
            return;
        }

        if (currentEmotion == Emotion.Happy)
        {
            if (missedSurprise)
                SetGiftState(GiftState.ExpectSurprise);
            else
                SetGiftState(GiftState.GiveGift);

            return;
        }

        if (emotionTimer <= 0f)
            SetGiftState(GiftState.Angry);
    }

    private void UpdateAngry()
    {
        LookAtPlayer();
        SetFace(FaceType.Angry);

        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) <= stats.attackRange)
            SetGiftState(GiftState.Attack);
    }

    private void UpdateAttackGift()
    {
        LookAtPlayer();
        AttackBehavior(); // EnemyBase Attack
        SetGiftState(GiftState.Leave);
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
            case FaceType.Surprised:     lookSurprised.SetActive(true); break;
            case FaceType.Disappointed:  lookDisappointed.SetActive(true); break;
            case FaceType.ExpectSmile:   lookExpectSmile.SetActive(true); break;
            case FaceType.Angry:         lookAngry.SetActive(true); break;
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
