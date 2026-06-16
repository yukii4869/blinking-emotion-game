using UnityEngine;
using System;

public class EmotionCondition : MonoBehaviour
{
    [SerializeField] private Transform scannerTarget;
    [SerializeField] private Emotion requiredEmotion;
    [SerializeField] private float scanDuration = 3.2f;
    [SerializeField] private float preDelay = 1.5f; // <<< Vorlaufzeit
    [SerializeField] private float resultDisplayTime = 2f;

    [SerializeField] private Renderer displayRenderer;

    // Materialien pro Emotion
    [SerializeField] private Material happyMaterial;
    [SerializeField] private Material sadMaterial;
    [SerializeField] private Material angryMaterial;
    [SerializeField] private Material surprisedMaterial;

    // Ergebnis-Materialien
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material successMaterial;
    [SerializeField] private Material failMaterial;
    [SerializeField] private Light doorLight;

    // Scanner
    [SerializeField] private GameObject scanCube;
    private bool inScan = false;
    private float scanTimer;
    private bool emotionWasCorrect = false;

    private bool showingResult = false;
    private float resultTimer;


    private float delayTimer;
    private Action onSuccess;
    private Action onFail;

    private bool running = false;
    private bool inDelay = false;
    public bool IsRunning => running || inDelay || inScan || showingResult;

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

    public void StartCondition(Action success, Action fail)
    {
        running = false;
        inDelay = false;

        onSuccess = success;
        onFail = fail;

        // Display zeigt die geforderte Emotion
        Material emotionMat = GetEmotionMaterial(requiredEmotion);
        if (emotionMat != null)
            displayRenderer.material = emotionMat;

        // Scanner ausblenden
        scanCube.SetActive(false);

        // Vorlaufzeit starten
        delayTimer = preDelay;
        inDelay = true;
        running = true;

    }
    private bool IsLookingAtScanner()
    {
        Camera cam = Camera.main;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3))
        {
            if (hit.transform == scannerTarget)
                return true;

            // trifft ein Kind des Scanners?
            if (hit.transform.IsChildOf(scannerTarget))
                return true;
        }

        return false;
    }




    private void Update()
    {
        if (showingResult)
        {
            resultTimer -= Time.deltaTime;

            if (resultTimer <= 0f)
            {
                showingResult = false;
                displayRenderer.material = defaultMaterial;
                doorLight.color = Color.orange;
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

                // Scan starten
                inScan = true;
                scanTimer = scanDuration;
                emotionWasCorrect = true;

                scanCube.SetActive(true);
            }

            return;
        }

        // -------------------------
        // PHASE 2: Scan läuft
        // -------------------------
        // -------------------------
        // PHASE 2: Scan läuft
        // -------------------------
        if (inScan)
        {
            scanTimer -= Time.deltaTime;

            // Emotion MUSS durchgehend korrekt sein
            if (GameplayFaceInput.Instance.currentEmotion != requiredEmotion)
            {
                FailNow();
                return;
            }

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

            return;
        }
    }
    private void FailNow()
    {
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





