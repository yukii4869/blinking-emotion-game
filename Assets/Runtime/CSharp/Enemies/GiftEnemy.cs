using UnityEngine;

public class GiftGuest : EmotionEnemyBase
{
    private enum GiftState
    {
        Approach,
        OfferGift,
        ExpectSurprise,
        ExpectJoy,
        PrepareGift,
        GiveGift,
        Disappointed,
        Angry,
        Attack,
        PauseBeforeLeave,
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
    [SerializeField] private Animator ghostAnimation;
    [SerializeField] private float pauseDuration = 1.5f;
    [SerializeField] private float giftDelay = 1.5f;
    private float giftDelayTimer;
    private float pauseTimer;

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
            case GiftState.Approach: UpdateApproachGift(); break;
            case GiftState.OfferGift: UpdateOfferGift(); break;
            case GiftState.ExpectSurprise: UpdateExpectSurprise(); break;
            case GiftState.ExpectJoy: UpdateExpectJoy(); break;
            case GiftState.PrepareGift: UpdatePrepareGift(); break;   // NEU
            case GiftState.GiveGift: UpdateGiveGift(); break;
            case GiftState.Disappointed: UpdateDisappointed(); break;
            case GiftState.Angry: UpdateAngry(); break;
            case GiftState.Attack: UpdateAttackGift(); break;
            case GiftState.PauseBeforeLeave: UpdatePauseBeforeLeave(); break;
            case GiftState.Leave: UpdateLeave(); break;
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
        if (ghostAnimation != null)
        {
            ghostAnimation.Play("GhostIdle", 0, 0f);
            ghostAnimation.enabled = false;
        }
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
            // NICHT direkt Geschenk geben → erst vorbereiten
            giftDelayTimer = giftDelay;
            SetGiftState(GiftState.PrepareGift);
            return;
        }

        if (emotionTimer <= 0f)
            SetGiftState(GiftState.Disappointed);
    }

    private void UpdatePrepareGift()
    {
        LookAtPlayer();
        SetFace(FaceType.ExpectSmile);   // hält das „zufriedene“ Gesicht

        giftDelayTimer -= Time.deltaTime;

        if (giftDelayTimer <= 0f)
            SetGiftState(GiftState.GiveGift);
    }

    private void UpdateGiveGift()
    {
        GiveGiftToPlayer();
        pauseTimer = pauseDuration;
        SetGiftState(GiftState.PauseBeforeLeave);
    }

    private void UpdateLeave()
    {
        lookAngry.SetActive(false);
        lookExpectSmile.SetActive(false);
        lookSurprised.SetActive(false);
        lookDisappointed.SetActive(false);
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
        AttackBehavior();
        pauseTimer = pauseDuration;
        SetGiftState(GiftState.PauseBeforeLeave);
    }
    private void UpdatePauseBeforeLeave()
    {
        LookAtPlayer(); // hält Blickkontakt
        if (ghostAnimation != null)
            ghostAnimation.enabled = true;
        pauseTimer -= Time.deltaTime;

        if (pauseTimer <= 0f)
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
