using System.Collections;
using UnityEngine;

public class SadnessElevator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private float drainSpeed = 0.2f;
    [SerializeField] private Animator doorAnimation;
    [SerializeField] private GameObject elevatorUIPrefab;

    private float fillAmount = 0f;          // 0 = ganz unten, 1 = ganz oben
    private bool isSad = false;
    private bool playerInside = false;
    private bool doorsClosed = false;
    private bool doorsMoving = false;
    private bool AtBottom => fillAmount <= 0.01f;
    private bool AtTop => fillAmount >= 0.99f;
    private ElevatorState state = ElevatorState.Idle;

    private Vector3 startPos;
    private GameObject activeUI;

    enum ElevatorState
    {
        Idle,          // Türen offen
        DoorsClosing,
        Closed,        // Türen zu, Fahrmodus aktiv
        Moving,        // fährt
        Locked,        // Türen offen, keine Bewegung
        DoorsOpening
    }




    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void OnEnable()
    {
        InputSelector.Instance.ActiveInput.OnEmotionChanged += HandleEmotion;
    }

    private void OnDisable()
    {
        InputSelector.Instance.ActiveInput.OnEmotionChanged -= HandleEmotion;
    }

    private void HandleEmotion(Emotion e)
    {
        isSad = (e == Emotion.Sad);
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (state == ElevatorState.Closed)
            state = ElevatorState.Moving;

        if (state == ElevatorState.Moving)
        {
            if (isSad && fillAmount < 1f)
                fillAmount += fillSpeed * Time.deltaTime;

            if (!isSad && fillAmount > 0f)
                fillAmount -= drainSpeed * Time.deltaTime;

            fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);

            float y = Mathf.Lerp(0f, maxHeight, fillAmount);
            transform.localPosition = startPos + new Vector3(0, y, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
        other.transform.SetParent(transform);
        other.GetComponent<PlayerController>().IsOnMovingPlatform = true;

        if (activeUI == null)
        {
            Canvas canvas = GameObject.FindGameObjectWithTag("GameplayCanvas").GetComponent<Canvas>();
            activeUI = Instantiate(elevatorUIPrefab, canvas.transform);
            activeUI.GetComponent<ElevatorUI>().SetElevator(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
        other.transform.SetParent(null);
        other.GetComponent<PlayerController>().IsOnMovingPlatform = false;

        if (activeUI != null)
        {
            Destroy(activeUI);
            activeUI = null;
        }
    }

    private IEnumerator CloseDoors()
    {
        state = ElevatorState.DoorsClosing;
        doorsMoving = true;

        doorAnimation.SetTrigger("Close");
        yield return new WaitForSeconds(3f);

        doorsClosed = true;
        doorsMoving = false;
        state = ElevatorState.Closed;
    }

    private IEnumerator OpenDoors()
    {
        state = ElevatorState.DoorsOpening;
        doorsMoving = true;
        Debug.Log("Open Doors");

        doorAnimation.SetTrigger("Open");
        yield return new WaitForSeconds(3f);

        doorsClosed = false;
        doorsMoving = false;
        state = ElevatorState.Idle;
    }

    public void OnButtonPressed()
    {
        // 1. Türen offen → schließen
        if (state == ElevatorState.Idle)
        {
            StartCoroutine(CloseDoors());
            return;
        }

        // 2. Türen zu → öffnen, aber NUR oben oder unten
        if ((state == ElevatorState.Closed || state == ElevatorState.Moving)
            && (AtBottom || AtTop))
        {
            StartCoroutine(OpenDoors());
            state = ElevatorState.Locked;
            return;
        }

        // 3. Locked → wieder schließen
        if (state == ElevatorState.Locked)
        {
            StartCoroutine(CloseDoors());
            return;
        }

        // 4. Wenn man mitten drin ist → NICHTS tun
        Debug.Log("Button ignored → Elevator not at top or bottom");
    }



    public float GetFillAmount()
    {
        return fillAmount;
    }
    public bool CanPressButton()
    {
        // Button darf gedrückt werden, wenn:
        // - Idle (Türen offen)
        // - Locked (oben/unten, Türen offen)
        // - Closed (Türen zu, aber nicht in Bewegung)

        return state == ElevatorState.Idle
            || state == ElevatorState.Locked
            || state == ElevatorState.Closed
            || (state == ElevatorState.Closed || state == ElevatorState.Moving)
            && (AtBottom || AtTop);
    }

}
