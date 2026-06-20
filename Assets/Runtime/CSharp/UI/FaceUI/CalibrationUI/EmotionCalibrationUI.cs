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
    private bool phaseRunning = false;
    private bool calibrationStarted = false;
    private bool emoteCalibrationFinished = false;
    private EmotionCalibrationPhase currentUIPhase = EmotionCalibrationPhase.None;
    private bool waitingToStartPhase = false;
    private string pendingPrompt;
    private EmotionCalibrationPhase pendingPhase = EmotionCalibrationPhase.None;

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
        if (!calibrationStarted || emoteCalibrationFinished || phaseRunning)
            return;

        if (waitingToStartPhase)
        {
            waitingToStartPhase = false;
            StartCoroutine(RunPhase(pendingPhase));
            return;
        }

        PrepareNextPhase();
    }
    private void PrepareNextPhase()
    {
        commentText.text = "";

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
            promptText.text = "Identität bestätigt.";
            commentText.text = "Mitarbeiterprofil gespeichert.";
            emoteCalibrationFinished = true;
        }
    }
    private void ShowPhasePrompt(EmotionCalibrationPhase phase)
    {
        pendingPhase = phase;
        currentUIPhase = phase;
        waitingToStartPhase = true;

        progressBar.gameObject.SetActive(false);


        StopAllCoroutines();
        StartCoroutine(typewriter.TypeText(promptText, GetPrompt(phase)));
        commentText.text = "Bestätigung erforderlich";
    }

    private void OnRetry(InputAction.CallbackContext context)
    {
        if (!calibrationStarted || emoteCalibrationFinished || phaseRunning)
            return;

        if (currentUIPhase == EmotionCalibrationPhase.None)
            return;

        emotionCalibrator.StartReCalibrateEmotion(currentUIPhase);
        ShowPhasePrompt(currentUIPhase);
    }

    private IEnumerator RunPhase(EmotionCalibrationPhase phase)
    {
        if (phaseRunning)
            yield break;

        phaseRunning = true;
        currentUIPhase = phase;
        promptText.text = "Bitte halten...";
        commentText.text = "";
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(typewriter.TypeText(promptText, "Erfassung läuft..."));
        commentText.text = "";

        switch (phase)
        {
            case EmotionCalibrationPhase.Neutral:
                emotionCalibrator.StartNeutralCalibration();
                break;
            case EmotionCalibrationPhase.SmileMax:
                emotionCalibrator.StartSmileCalibration();
                break;
            case EmotionCalibrationPhase.AngryMax:
                emotionCalibrator.StartAngryCalibration();
                break;
            case EmotionCalibrationPhase.SadMax:
                emotionCalibrator.StartSadCalibration();
                break;
            case EmotionCalibrationPhase.SurprisedMax:
                emotionCalibrator.StartSurprisedCalibration();
                break;
        }

        yield return StartCoroutine(ProgressCalibration(phase));

        phaseRunning = false;
        yield return new WaitForSeconds(0.5f);
        progressBar.gameObject.SetActive(false);
        yield return StartCoroutine(typewriter.TypeText(promptText, GetSavedText(phase)));
        commentText.text = "Bestätigung erforderlich";
        commentText.text = "Aufnahme abgeschlossen \n Bestätigung erforderlich";
    }
    public void ResetUI()
    {
        calibrationStarted = false;
        emoteCalibrationFinished = false;
        phaseRunning = false;
        currentUIPhase = EmotionCalibrationPhase.None;

        progressBar.value = 0f;
        promptText.text = "Bereit zur Kalibrierung";
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
    private string GetPrompt(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral =>
                "AUSGANGSZUSTAND WIRD ERFASST\n\nBitte entspannen Sie Ihr Gesicht.",

            EmotionCalibrationPhase.SmileMax =>
                "FREUNDLICHKEIT WIRD ERFASST\n\nBitte zeigen Sie ein freundliches Mitarbeiterlächeln.",

            EmotionCalibrationPhase.AngryMax =>
                "AGGRESSIONSMUSTER WIRD ERFASST\n\nBitte zeigen Sie einen deutlich verärgerten Ausdruck.",

            EmotionCalibrationPhase.SadMax =>
                "EMOTIONALE BELASTUNG WIRD ERFASST\n\nBitte zeigen Sie einen deutlich traurigen Ausdruck.",

            EmotionCalibrationPhase.SurprisedMax =>
                "SCHRECKREAKTION WIRD ERFASST\n\nBitte zeigen Sie einen deutlich überraschten Ausdruck.",

            _ => ""
        };
    }
    private string GetSavedText(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral => "AUSGANGSZUSTAND GESPEICHERT",
            EmotionCalibrationPhase.SmileMax => "FREUNDLICHKEITSPROFIL GESPEICHERT",
            EmotionCalibrationPhase.AngryMax => "AGGRESSIONSMUSTER GESPEICHERT",
            EmotionCalibrationPhase.SadMax => "EMOTIONALE BELASTUNG GESPEICHERT",
            EmotionCalibrationPhase.SurprisedMax => "SCHRECKREAKTION GESPEICHERT",
            _ => "AUFNAHME GESPEICHERT"
        };
    }
}
