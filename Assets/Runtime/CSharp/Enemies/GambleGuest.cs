using System.Collections;
using UnityEngine;

public class GambleGuest : EnemyBase
{
    public float approachDistance = 2f;
    public Animator anim;
    public GameObject uiPrefab;
    private GameObject activeUI;
    private Transform uiRoot;
    public GameObject ritualCirclePrefab;
    private GameObject ritualCircleInstance;
    public override void Start()
    {
        base.Start();
        uiRoot = GameObject.FindGameObjectWithTag("GameplayCanvas").transform;
    }

    private int chosenCard = -1;

    private void OnEnable()
    {
        GameplayFaceInput.OnEyesClosed += HandleEyesClosed;
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnEyesClosed -= HandleEyesClosed;
    }

    private void HandleEyesClosed()
    {
        if (CurrentState == EnemyState.WaitingForEyes)
            SetState(EnemyState.Resolve);
    }

    public override void UpdateBehavior()
    {
        if (stunned) return;

        switch (CurrentState)
        {
            case EnemyState.Idle:
                ApproachPlayer();
                if (Vector3.Distance(transform.position, player.transform.position) <= approachDistance)
                {
                    agent.ResetPath();
                    SpawnRitualCircle();
                    SetState(EnemyState.Interact);
                }
                break;


            case EnemyState.Interact:
                anim.SetTrigger("OpenDeck");

                // UI erzeugen
                if (activeUI == null)
                    activeUI = Instantiate(uiPrefab, uiRoot);

                activeUI.SetActive(false); // erst später anzeigen

                SetState(EnemyState.WaitingForCard);
                break;

            case EnemyState.WaitingForCard:
                if (chosenCard != -1)
                {
                    anim.SetTrigger("CloseDeck");

                    // UI anzeigen
                    activeUI.SetActive(true);

                    SetState(EnemyState.WaitingForEyes);
                }
                break;
            case EnemyState.WaitingForEyes:
                // Wir warten NUR auf das Event → kein Code hier
                break;

            case EnemyState.Resolve:
                if (activeUI != null)
                    activeUI.SetActive(false);

                bool good = Random.value > 0.5f;

                if (good)
                    Debug.Log("GOOD OUTCOME");
                else
                    Debug.Log("BAD OUTCOME");

                // Optional: Sound nach 3 Sekunden
                StartCoroutine(PlayRevealSound());
                Cleanup();
                SetState(EnemyState.GoingHome);
                chosenCard = -1;
                break;
            case EnemyState.GoingHome:
                GoToRoom();
                break;


        }
    }

    public void OnCardChosen(int index)
    {
        if (CurrentState != EnemyState.WaitingForCard)
            return;

        chosenCard = index;
    }

    private IEnumerator PlayRevealSound()
    {
        yield return new WaitForSeconds(3f);
        // Hier Sound abspielen
        Debug.Log("Reveal Sound!");
    }
    private void OnDestroy()
    {
        if (activeUI != null)
            Destroy(activeUI);
    }
    private void SpawnRitualCircle()
    {
        if (ritualCircleInstance != null) return;

        ritualCircleInstance = Instantiate(ritualCirclePrefab, transform.position, Quaternion.identity);

        RitualCircle circle = ritualCircleInstance.GetComponent<RitualCircle>();
        //circle.center = transform;
        circle.radius = 2.5f;
    }
    private void Cleanup()
    {
        if (ritualCircleInstance != null)
            Destroy(ritualCircleInstance);
    }
}
