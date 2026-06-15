using UnityEngine;
using System;

public class EmotionCondition : MonoBehaviour
{
    [SerializeField] private Emotion requiredEmotion;
    [SerializeField] private float scanDuration = 3.2f;
    [SerializeField] private float preDelay = 1.5f; // <<< Vorlaufzeit

    [SerializeField] private Renderer displayRenderer;

    // Materialien pro Emotion
    [SerializeField] private Material happyMaterial;
    [SerializeField] private Material sadMaterial;
    [SerializeField] private Material angryMaterial;
    [SerializeField] private Material surprisedMaterial;

    // Ergebnis-Materialien
    [SerializeField] private Material successMaterial;
    [SerializeField] private Material failMaterial;
    [SerializeField] private Light doorLight;

    // Scanner
    [SerializeField] private GameObject scanCube;
    private bool inScan = false;
    private float scanTimer;
    private bool emotionWasCorrect = false;


    private float delayTimer;
    private Action onSuccess;
    private Action onFail;

    private bool running = false;
    private bool inDelay = false;

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


    private void Update()
    {
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
                emotionWasCorrect = true; // Start: assume correct until proven wrong

                scanCube.SetActive(true);
            }

            return;
        }

        // -------------------------
        // PHASE 2: Scan läuft
        // -------------------------
        if (inScan)
        {
            scanTimer -= Time.deltaTime;

            // Emotion MUSS durchgehend korrekt sein
            if (GameplayFaceInput.Instance.currentEmotion != requiredEmotion)
            {
                // Sofort FAIL
                inScan = false;
                running = false;

                scanCube.SetActive(false);
                displayRenderer.material = failMaterial;
                doorLight.color = Color.red;
                onFail?.Invoke();
                return;
            }

            // Scan fertig → SUCCESS
            if (scanTimer <= 0f)
            {
                inScan = false;
                running = false;

                scanCube.SetActive(false);
                doorLight.color = Color.green;
                displayRenderer.material = successMaterial;
                onSuccess?.Invoke();
            }

            return;
        }
    }

}
