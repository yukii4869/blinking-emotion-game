using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GambleGuest : EnemyBase
{
    private enum GambleState
    {
        Approach,
        Interact,
        WaitingForCard,
        WaitingForEyes,
        Resolve
    }

    public float approachDistance = 2f;
    public Animator anim;

    [Header("UI")]
    public GameObject uiPrefab;
    private GameObject activeUI;
    private Transform uiRoot;

    [Header("Ritual")]
    [SerializeField] private GameObject ritualCirclePrefab;
    
    [SerializeField] private GameObject giftItemPrefab;
    private GameObject ritualCircleInstance;

    private GambleState gambleState = GambleState.Approach;
    private int chosenCard = -1;

    public override void Start()
    {
        base.Start();
        uiRoot = GameObject.FindGameObjectWithTag("GameplayCanvas").transform;

        gambleState = GambleState.Approach;
        SetState(EnemyState.Special); // WICHTIG: alles läuft über Special
    }

    private void OnEnable()
    {
        GameplayFaceInput.OnEyesClosedHold += HandleEyesClosed;
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnEyesClosedHold -= HandleEyesClosed;
    }

    private void HandleEyesClosed()
    {
        if (gambleState == GambleState.WaitingForEyes)
            gambleState = GambleState.Resolve;
    }

    // ---------------------------------------------------------
    // SPECIAL LOGIC
    // ---------------------------------------------------------
    protected override void UpdateSpecial()
    {
        switch (gambleState)
        {
            case GambleState.Approach:
                LookAtPlayer();
                UpdateApproachGamble();
                break;

            case GambleState.Interact:
                LookAtPlayer();
                UpdateInteract();
                break;

            case GambleState.WaitingForCard:
                LookAtPlayer();
                UpdateWaitingForCard();
                break;

            case GambleState.WaitingForEyes:
                LookAtPlayer();
                // wartet nur auf Event
                break;

            case GambleState.Resolve:
                UpdateResolve();
                break;
        }
    }

    // ---------------------------------------------------------
    // STATE LOGIC
    // ---------------------------------------------------------

    private void UpdateApproachGamble()
    {
        ApproachPlayer();

        if (Vector3.Distance(transform.position, player.transform.position) <= approachDistance)
        {
            agent.ResetPath();
            SpawnRitualCircle();
            gambleState = GambleState.Interact;
        }
    }

    private void UpdateInteract()
    {
        anim.SetTrigger("OpenDeck");

        if (activeUI == null)
            activeUI = Instantiate(uiPrefab, uiRoot);

        activeUI.SetActive(false);

        gambleState = GambleState.WaitingForCard;
    }

    private void UpdateWaitingForCard()
    {
        if (chosenCard == -1)
            return;

        anim.SetTrigger("CloseDeck");
        activeUI.SetActive(true);

        gambleState = GambleState.WaitingForEyes;
    }

    private void UpdateResolve()
    {
        if (activeUI != null)
            activeUI.SetActive(false);

        bool good = Random.value > 0.5f;
        Debug.Log(good ? "GOOD OUTCOME" : "BAD OUTCOME");

        StartCoroutine(PlayRevealSound());
        if (good)
        {
            DoGoodOutcome();
        }
        else
        {
            DoBadOutCome();
        }
        Cleanup();

        chosenCard = -1;

        SetState(EnemyState.GoingHome); // EnemyBase übernimmt Heimweg
    }
    private void DoBadOutCome()
    {
        AttackBehavior();
        Debug.Log("Attacke");
        PlayerHealth.Instance.TakeDamage(stats.damage);
    }
    private void DoGoodOutcome()
    {
        Instantiate(giftItemPrefab, transform.position + transform.forward, Quaternion.identity);
    }

    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    public void OnCardChosen(int index)
    {
        if (gambleState != GambleState.WaitingForCard)
            return;

        chosenCard = index;
    }

    private IEnumerator PlayRevealSound()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Reveal Sound!");
    }

    private void SpawnRitualCircle()
    {
        if (ritualCircleInstance != null) return;

        ritualCircleInstance = Instantiate(ritualCirclePrefab, transform.position, Quaternion.identity);

        RitualCircle circle = ritualCircleInstance.GetComponent<RitualCircle>();
        circle.radius = 2.5f;
    }

    private void Cleanup()
    {
        if (ritualCircleInstance != null)
            Destroy(ritualCircleInstance);
    }

    private void OnDestroy()
    {
        if (activeUI != null)
            Destroy(activeUI);
    }
    public bool CanInteract()
    {
        return !(gambleState == GambleState.WaitingForCard);
    }
}
