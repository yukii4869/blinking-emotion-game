using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class EmotionCalibrationUI : MonoBehaviour
{
    [SerializeField] private EmotionCalibrator emotionCalibrator;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private EARCalibrator eARCalibrator;
    [SerializeField] private InputActionReference continueAction;
    [SerializeField] private InputActionReference retryAction;
    [SerializeField] private Typewriter typewriter;
    [SerializeField] private TextMeshProUGUI interactionHintText;

    private bool skipRequested = false;
    private bool allowRetry = false;
    private bool isTyping = false;

    private bool phaseRunning = false;
    private bool calibrationStarted = false;
    private bool emoteCalibrationFinished = false;

    private EmotionCalibrationPhase currentUIPhase = EmotionCalibrationPhase.None;
    private EmotionCalibrationPhase pendingPhase = EmotionCalibrationPhase.None;

    private bool waitingToStartPhase = false;

    // SPAM-SCHUTZ
    private bool transitionLock = false;

    private void Update()
    {
        if (CalibrationStateManager.Instance.CurrentState != CalibrationState.EmotionCalibration)
            return;

        if (!calibrationStarted && !emoteCalibrationFinished)
        {
            calibrationStarted = true;
            progressBar.value = 0f;
        }
    }

    private void OnEnable()
    {
        continueAction.action.Enable();
        retryAction.action.Enable();

        continueAction.action.performed += OnContinue;
        retryAction.action.performed += OnRetry;
    }

    private void OnDisable()
    {
        continueAction.action.performed -= OnContinue;
        retryAction.action.performed -= OnRetry;

        continueAction.action.Disable();
        retryAction.action.Disable();
    }

    private void OnContinue(InputAction.CallbackContext context)
    {
        // Skip während Typewriter
        if (isTyping)
        {
            skipRequested = true;
            return;
        }

        // Blockieren, wenn wir in einem Übergang sind
        if (transitionLock)
            return;

        if (!calibrationStarted || emoteCalibrationFinished || phaseRunning)
            return;

        // Phase starten
        if (waitingToStartPhase && pendingPhase != EmotionCalibrationPhase.None)
        {
            waitingToStartPhase = false;
            transitionLock = true;
            StartCoroutine(RunPhase(pendingPhase));
            return;
        }

        // Nächste Phase vorbereiten
        PrepareNextPhase();
    }

    private void PrepareNextPhase()
    {
        commentText.text = "";
        interactionHintText.text = "";

        if (!IsPhaseFinished(EmotionCalibrationPhase.Neutral))
            ShowPhasePrompt(EmotionCalibrationPhase.Neutral);
        else if (!IsPhaseFinished(EmotionCalibrationPhase.SmileMax))
            ShowPhasePrompt(EmotionCalibrationPhase.SmileMax);
        else if (!IsPhaseFinished(EmotionCalibrationPhase.AngryMax))
            ShowPhasePrompt(EmotionCalibrationPhase.AngryMax);
        else if (!IsPhaseFinished(EmotionCalibrationPhase.SadMax))
            ShowPhasePrompt(EmotionCalibrationPhase.SadMax);
        else if (!IsPhaseFinished(EmotionCalibrationPhase.SurprisedMax))
            ShowPhasePrompt(EmotionCalibrationPhase.SurprisedMax);
        else
        {
            emotionCalibrator.StartComputeGlobalMax();
            promptText.text = "Die aufgezeichneten Muster entsprechen Ihrem Profil.\n\n Bitte bestätigen Sie, dass Sie sich darin erkennen\n Falls Sie das Gefühl haben, dass etwas nicht stimmt, empfehlen wir eine erneute Erfassung.";
            commentText.text = "Bitte bestätigen";
            emoteCalibrationFinished = true;
        }
    }

    private void ShowPhasePrompt(EmotionCalibrationPhase phase)
    {
        allowRetry = false;

        pendingPhase = phase;
        currentUIPhase = phase;
        waitingToStartPhase = true;

        progressBar.gameObject.SetActive(false);

        // KEIN transitionLock hier → sonst blockiert Skip
        StartCoroutine(RunTypewriter(GetPrompt(phase)));
        StartCoroutine(ShowConfirmAfterTyping());
    }

    private void OnRetry(InputAction.CallbackContext context)
    {
        if (transitionLock)
            return;

        if (!calibrationStarted || emoteCalibrationFinished || phaseRunning)
            return;

        if (currentUIPhase == EmotionCalibrationPhase.None)
            return;

        emotionCalibrator.StartReCalibrateEmotion(currentUIPhase);
        ShowPhasePrompt(currentUIPhase);
    }

    private IEnumerator RunPhase(EmotionCalibrationPhase phase)
    {
        phaseRunning = true;
        transitionLock = true;

        commentText.text = "";
        interactionHintText.text = "";

        promptText.text = "Bereit machen...";
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(RunTypewriter("Erfassung läuft..."));

        switch (phase)
        {
            case EmotionCalibrationPhase.Neutral: emotionCalibrator.StartNeutralCalibration(); break;
            case EmotionCalibrationPhase.SmileMax: emotionCalibrator.StartSmileCalibration(); break;
            case EmotionCalibrationPhase.AngryMax: emotionCalibrator.StartAngryCalibration(); break;
            case EmotionCalibrationPhase.SadMax: emotionCalibrator.StartSadCalibration(); break;
            case EmotionCalibrationPhase.SurprisedMax: emotionCalibrator.StartSurprisedCalibration(); break;
        }

        yield return StartCoroutine(ProgressCalibration(phase));

        phaseRunning = false;
        yield return new WaitForSeconds(0.5f);

        progressBar.gameObject.SetActive(false);

        yield return StartCoroutine(RunTypewriter(GetSavedText(phase)));

        allowRetry = true;
        StartCoroutine(ShowConfirmAfterTyping());
    }

    public void ResetUI()
    {
        calibrationStarted = false;
        emoteCalibrationFinished = false;
        phaseRunning = false;

        currentUIPhase = EmotionCalibrationPhase.None;
        pendingPhase = EmotionCalibrationPhase.None;

        waitingToStartPhase = false;
        allowRetry = false;
        skipRequested = false;
        isTyping = false;
        transitionLock = false;

        progressBar.value = 0f;
        progressBar.gameObject.SetActive(false);

        promptText.text = "Ihre Entscheidung zur Wiederholung wurde bestätigt.";
        commentText.text = "Bestätigung erforderlich";
        interactionHintText.text = "[E] Bestätigen";
    }

    private IEnumerator ProgressCalibration(EmotionCalibrationPhase phase)
    {
        progressBar.gameObject.SetActive(true);
        progressBar.value = 0f;

        float minDuration = 1.2f;
        float timer = 0f;

        while (!IsPhaseFinished(phase) || timer < minDuration)
        {
            timer += Time.deltaTime;
            progressBar.value = (float)emotionCalibrator.collectedFrames / emotionCalibrator.maxFrames;
            yield return null;
        }
    }

    private bool IsPhaseFinished(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral => emotionCalibrator.finishedNeutral,
            EmotionCalibrationPhase.SmileMax => emotionCalibrator.finishedSmile,
            EmotionCalibrationPhase.AngryMax => emotionCalibrator.finishedAngry,
            EmotionCalibrationPhase.SadMax => emotionCalibrator.finishedSad,
            EmotionCalibrationPhase.SurprisedMax => emotionCalibrator.finishedSurprised,
            _ => false
        };
    }

    private IEnumerator RunTypewriter(string text)
    {
        isTyping = true;
        skipRequested = false;

        promptText.text = "";
        commentText.text = "";
        interactionHintText.text = "";

        float delay = 1f / typewriter.charsPerSecond;

        foreach (char c in text)
        {
            if (skipRequested)
            {
                promptText.text = text;
                break;
            }

            promptText.text += c;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    private IEnumerator ShowConfirmAfterTyping()
    {
        yield return new WaitUntil(() => !isTyping);

        commentText.text = "Bestätigung erforderlich";

        if (allowRetry)
            interactionHintText.text = "[E] Bestätigen   [R] Wiederholen";
        else
            interactionHintText.text = "[E] Bestätigen";

        transitionLock = false;
    }

    private string GetPrompt(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral =>
                "Vorbereitung: \nBitte entspannen Sie Ihr Gesicht.\n\n \nDie Erfassung beginnt erst nach Ihrer Bestätigung.",

            EmotionCalibrationPhase.SmileMax =>
                "Vorbereitung:\nBitte bereiten Sie ein freundliches Mitarbeiterlächeln vor.",

            EmotionCalibrationPhase.AngryMax =>
                "Vorbereitung: \nBitte rufen Sie einen verärgerten Ausdruck hervor.",

            EmotionCalibrationPhase.SadMax =>
                "Vorbereitung: \nBitte bereiten Sie einen traurigen Ausdruck vor.",

            EmotionCalibrationPhase.SurprisedMax =>
                "Vorbereitung: \nBitte bereiten Sie eine überraschte Reaktion vor.",

            _ => ""
        };
    }

    private string GetSavedText(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral =>
                "Ihr natürlicher Zustand wurde festgehalten.\n\n\nEr entspricht den Erwartungen.",

            EmotionCalibrationPhase.SmileMax =>
                "Ihr Lächeln wurde erfasst.\n\n\nEs wirkt gestellt.",

            EmotionCalibrationPhase.AngryMax =>
                "Ihr Ärger wurde deutlich registriert.\n\n\nDie Intensität ist bemerkenswert.",

            EmotionCalibrationPhase.SadMax =>
                "Ihre Traurigkeit wurde aufgezeichnet.\n\n\nSie scheint...angemessen.",

            EmotionCalibrationPhase.SurprisedMax =>
                "Ihre Schreckreaktion wurde eingefangen.\n\n\nSie war in Ordnung.",

            _ => "Die Reaktion wurde aufgezeichnet."
        };
    }

}
