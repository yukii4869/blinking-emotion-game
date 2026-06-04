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
    [SerializeField] private Transform roomPosition;
    [SerializeField] private GameObject handGiftObject;

    private Vector3 leaveTarget;

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
            case GiftState.Leave:
                UpdateLeave();
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
                armAnimator.SetTrigger("OfferGift");
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
                Debug.Log("[GiftGuest] State: Leave");
                UpdateLeave();
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

        if (dist <= stats.stopDistance + 1f)
        {
            SetState(GiftState.OfferGift);
        }
    }

    private void UpdateOfferGift()
    {
        Debug.Log("[GiftGuest] State: OfferGift");
        LookAtPlayer();
        SetFace(FaceType.Surprised);
        if (armAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            SetState(GiftState.ExpectSurprise);
        }
    }

    private void UpdateExpectSurprise()
    {
        Debug.Log("[GiftGuest] State: ExpectSurprise");
        LookAtPlayer();
        SetFace(FaceType.Surprised);
        emotionTimer -= Time.deltaTime;

        if (currentEmotion == Emotion.Surprised)
        {
            Debug.Log("[GiftGuest] Surprise detected! Moving to ExpectJoy.");
            missedSurprise = false;
            SetState(GiftState.ExpectJoy);
            return;
        }

        if (emotionTimer <= 0f)
        {
            Debug.Log("[GiftGuest] Surprise NOT detected → Disappointed");
            missedSurprise = true;
            SetState(GiftState.Disappointed);
        }
    }

    private void UpdateExpectJoy()
    {
        Debug.Log("[GiftGuest] State: ExpectJoy");
        LookAtPlayer();
        SetFace(FaceType.ExpectSmile);
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
        LookAtPlayer();
        SetFace(FaceType.Disappointed);
        emotionTimer -= Time.deltaTime;

        // Spieler macht Surprise
        if (currentEmotion == Emotion.Surprised)
        {
            Debug.Log("[GiftGuest] Surprise in Disappointed");

            // Egal ob Surprise oder Joy verpasst wurde:
            // Surprise = "Ich hab's verstanden"
            SetState(GiftState.ExpectJoy);
            return;
        }

        // Spieler macht Happy
        if (currentEmotion == Emotion.Happy)
        {
            Debug.Log("[GiftGuest] Happy in Disappointed");

            if (missedSurprise)
            {
                // Spieler hat Surprise verpasst → Happy reicht NICHT
                // Er muss Surprise nachholen
                SetState(GiftState.ExpectSurprise);
            }
            else
            {
                // Spieler hat Joy verpasst → Happy = Quest geschafft
                SetState(GiftState.GiveGift);
            }

            return;
        }

        // Wenn Zeit abläuft → endgültig wütend
        if (emotionTimer <= 0f)
        {
            Debug.Log("[GiftGuest] Disappointment turned into ANGER!");
            SetState(GiftState.Angry);
        }
    }


    private void UpdateAngry()
    {
        Debug.Log("[GiftGuest] State: Angry");
        SetFace(FaceType.Angry);

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
        SetState(GiftState.Leave);
    }
    private void UpdateLeave()
    {
        agent.stoppingDistance = 0f;
        agent.speed = stats.moveSpeed * 0.8f;
        agent.SetDestination(roomPosition.position);

        float dist = Vector3.Distance(transform.position, roomPosition.position);

        if (dist < 0.3f)
        {
            Debug.Log("[GiftGuest] Reached room → despawn");
            Destroy(gameObject);
        }
    }



    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    private void GiveGiftToPlayer()
    {
        Debug.Log("[GiftGuest] Giving gift to player.");

        // Hand-Geschenk ausblenden
        if (handGiftObject != null)
            handGiftObject.SetActive(false);

        // Neues Geschenk instanziieren
        Instantiate(giftItemPrefab, transform.position + transform.forward, Quaternion.identity);

        // Danach in Leave-State
        SetState(GiftState.Leave);
    }
    private void SetFace(FaceType face)
    {
        lookAngry.SetActive(false);
        lookExpectSmile.SetActive(false);
        lookSurprised.SetActive(false);
        lookDisappointed.SetActive(false);

        switch (face)
        {
            case FaceType.Surprised:
                lookSurprised.SetActive(true);
                break;
            case FaceType.Disappointed:
                lookDisappointed.SetActive(true);
                break;
            case FaceType.ExpectSmile:
                lookExpectSmile.SetActive(true);
                break;
            case FaceType.Angry:
                lookAngry.SetActive(true);
                break;
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
