using UnityEngine;
using System;

public class EmotionCondition : MonoBehaviour
{
    public enum ConditionType
    {
        Emotion,
        NoBlink
    }

    [Header("Condition Type")]
    [SerializeField] private ConditionType conditionType = ConditionType.Emotion;

    [Header("Emotion Settings")]
    [SerializeField] private Emotion requiredEmotion;

    [Header("Timing")]
    [SerializeField] private float scanDuration = 3.2f;
    [SerializeField] private float preDelay = 1.5f;
    [SerializeField] private float resultDisplayTime = 2f;

    [Header("Scanner")]
    [SerializeField] private Transform scannerTarget;
    [SerializeField] private GameObject scanCube;

    [Header("Display")]
    [SerializeField] private Renderer displayRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material successMaterial;
    [SerializeField] private Material failMaterial;

    [Header("Emotion Materials")]
    [SerializeField] private Material happyMaterial;
    [SerializeField] private Material sadMaterial;
    [SerializeField] private Material angryMaterial;
    [SerializeField] private Material surprisedMaterial;

    [Header("No Blink Material")]
    [SerializeField] private Material noBlinkMaterial;

    [Header("Door Light")]
    [SerializeField] private Light doorLight;

    private bool running = false;
    private bool inDelay = false;
    private bool inScan = false;
    private bool showingResult = false;

    private float delayTimer;
    private float scanTimer;
    private float resultTimer;

    private Action onSuccess;
    private Action onFail;

    private bool emotionFailed = false;
    private bool blinkFailed = false;

    private InputBase activeInput;   // NEU

    public bool IsRunning => running || inDelay || inScan || showingResult;

    // ---------------------------------------------------------
    // MATERIAL SELECTION
    // ---------------------------------------------------------

    private Material GetEmotionMaterial(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Happy: return happyMaterial;
            case Emotion.Sad: return sadMaterial;
            case Emotion.Angry: return angryMaterial;
            case Emotion.Surprised: return surprisedMaterial;
        }
        return null;
    }

    // ---------------------------------------------------------
    // START CONDITION
    // ---------------------------------------------------------

    public void StartCondition(Action success, Action fail)
    {
        running = false;
        inDelay = false;
        emotionFailed = false;
        blinkFailed = false;

        onSuccess = success;
        onFail = fail;

        activeInput = InputSelector.Instance.ActiveInput;   // NEU

        // Display zeigt die geforderte Emotion oder NoBlink
        Material mat = null;

        if (conditionType == ConditionType.Emotion)
            mat = GetEmotionMaterial(requiredEmotion);

        if (conditionType == ConditionType.NoBlink)
            mat = noBlinkMaterial;

        displayRenderer.material = mat;

        scanCube.SetActive(false);

        delayTimer = preDelay;
        inDelay = true;
        running = true;

        // EVENTS ABONNIEREN (NEU)
        activeInput.OnBlink += HandleBlink;
        activeInput.OnEmotionChanged += HandleEmotionChanged;
    }


    // ---------------------------------------------------------
    // EVENT HANDLERS
    // ---------------------------------------------------------

    private void HandleBlink()
    {
        if (!inScan) return;

        if (conditionType == ConditionType.NoBlink)
        {
            blinkFailed = true;
            FailNow();
        }
    }

    private void HandleEmotionChanged(Emotion e)
    {
        if (!inScan) return;

        if (conditionType == ConditionType.Emotion)
        {
            if (e != requiredEmotion)
            {
                emotionFailed = true;
                FailNow();
            }
        }
    }

    private void CleanupEvents()
    {
        if (activeInput == null) return;

        activeInput.OnBlink -= HandleBlink;
        activeInput.OnEmotionChanged -= HandleEmotionChanged;
    }

    // ---------------------------------------------------------
    // LOOK CHECK
    // ---------------------------------------------------------

    private bool IsLookingAtScanner()
    {
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3))
        {
            if (hit.transform == scannerTarget)
                return true;

            if (hit.transform.IsChildOf(scannerTarget))
                return true;
        }

        return false;
    }

    // ---------------------------------------------------------
    // UPDATE LOOP
    // ---------------------------------------------------------

    private void Update()
    {
        // Ergebnis anzeigen
        if (showingResult)
        {
            resultTimer -= Time.deltaTime;

            if (resultTimer <= 0f)
            {
                showingResult = false;
                displayRenderer.material = defaultMaterial;
                doorLight.color = Color.yellow;
            }
            return;
        }

        if (!running) return;

        // -------------------------
        // PHASE 1: Vorlaufzeit
        // -------------------------
        if (inDelay)
        {
            delayTimer -= Time.deltaTime;

            if (delayTimer <= 0f)
            {
                inDelay = false;
                inScan = true;
                scanTimer = scanDuration;
                scanCube.SetActive(true);

                Emotion current = activeInput.currentEmotion;

                if (conditionType == ConditionType.Emotion)
                {
                    if (current != requiredEmotion)
                    {
                        FailNow();
                        return;
                    }
                }
            }
            return;
        }

        // -------------------------
        // PHASE 2: Scan läuft
        // -------------------------
        if (inScan)
        {
            scanTimer -= Time.deltaTime;

            // Spieler MUSS den Scanner anschauen
            if (!IsLookingAtScanner())
            {
                FailNow();
                return;
            }

            // Scan fertig → SUCCESS
            if (scanTimer <= 0f)
            {
                SuccessNow();
            }
        }
    }

    // ---------------------------------------------------------
    // FAIL / SUCCESS
    // ---------------------------------------------------------

    private void FailNow()
    {
        AudioManager.Instance.PlaySFX("failDoor");
        CleanupEvents();

        inScan = false;
        running = false;

        scanCube.SetActive(false);
        displayRenderer.material = failMaterial;
        doorLight.color = Color.red;

        showingResult = true;
        resultTimer = resultDisplayTime;

        onFail?.Invoke();
    }

    private void SuccessNow()
    {
        AudioManager.Instance.PlaySFX("correctDoor");
        CleanupEvents();

        inScan = false;
        running = false;

        scanCube.SetActive(false);
        displayRenderer.material = successMaterial;
        doorLight.color = Color.green;

        showingResult = true;
        resultTimer = resultDisplayTime;

        onSuccess?.Invoke();
    }
}
