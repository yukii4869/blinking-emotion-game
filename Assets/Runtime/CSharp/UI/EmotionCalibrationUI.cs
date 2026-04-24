using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class EmotionCalibrationUI : MonoBehaviour
{
    [SerializeField] private EmotionCalibrator emotionCalibrator;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private EARCalibrator eARCalibrator;
    [SerializeField] GameObject okayButton;
    [SerializeField] GameObject retryButton;
    private bool phaseRunning = false;
    private bool calibrationStarted = false;
    private bool emoteCalibrationFinished = false;
    private EmotionCalibrationPhase currentUIPhase = EmotionCalibrationPhase.None;

    private void Update()
    {
        if (!eARCalibrator.finishedCalibration)
            return;
        if (!calibrationStarted && !emoteCalibrationFinished)
        {
            calibrationStarted = true;
            progressBar.value = 0f;
            okayButton.SetActive(true);
            retryButton.SetActive(true);
        }

    }
    public void StartCalibrationEmotion()
    {
        if (phaseRunning)
        {
            return;
        }

        if (!IsPhaseFinished(EmotionCalibrationPhase.Neutral))
        {
            StartCoroutine(RunPhase("Schau neutral", EmotionCalibrationPhase.Neutral));
            retryButton.SetActive(true);
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.Neutral) && !IsPhaseFinished(EmotionCalibrationPhase.SmileMax))
        {
            StartCoroutine(RunPhase("Zeig dein schoenstes Laecheln", EmotionCalibrationPhase.SmileMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.SmileMax) && !IsPhaseFinished(EmotionCalibrationPhase.AngryMax))
        {
            StartCoroutine(RunPhase("Schau richtig wuetend", EmotionCalibrationPhase.AngryMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.AngryMax) && !IsPhaseFinished(EmotionCalibrationPhase.SadMax))
        {
            StartCoroutine(RunPhase("Schau richtig traurig", EmotionCalibrationPhase.SadMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.SadMax) && !IsPhaseFinished(EmotionCalibrationPhase.SurprisedMax))
        {
            StartCoroutine(RunPhase("Schau ueberrascht", EmotionCalibrationPhase.SurprisedMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.SurprisedMax))
        {
            emotionCalibrator.StartComputeGlobalMax();
            promptText.text = "Kalibrierung abgeschlossen!";
            emoteCalibrationFinished = true;
            return;
        }
    }
    public void StartReCalibrationEmotion()
    {
        if (phaseRunning|| currentUIPhase == EmotionCalibrationPhase.None)
        {
            return;
        }
        

        // 1. Emotion zurücksetzen
        emotionCalibrator.StartReCalibrateEmotion(currentUIPhase);

        // 2. Phase erneut starten
        StartCoroutine(RunPhase(GetPrompt(currentUIPhase), currentUIPhase));
    }
    private IEnumerator RunPhase(string prompt, EmotionCalibrationPhase phase)
    {
        if (phaseRunning)
        {
            yield break;// verhindert Doppelstart
        }
        phaseRunning = true;
        currentUIPhase = phase;
        promptText.text = prompt;

        yield return StartCoroutine(Countdown());

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


    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1f);
        promptText.text = "3";
        yield return new WaitForSeconds(1f);
        promptText.text = "2";
        yield return new WaitForSeconds(1f);
        promptText.text = "1";
        yield return new WaitForSeconds(1f);
        promptText.text = "Go!";
    }
    private IEnumerator ProgressCalibration(EmotionCalibrationPhase phase)
    {
        progressBar.value = 0f;

        while (!IsPhaseFinished(phase))
        {
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
            EmotionCalibrationPhase.Neutral => "Schau neutral",
            EmotionCalibrationPhase.SmileMax => "Zeig dein schönstes Lächeln",
            EmotionCalibrationPhase.AngryMax => "Schau richtig wütend",
            EmotionCalibrationPhase.SadMax => "Schau traurig",
            EmotionCalibrationPhase.SurprisedMax => "Schau überrascht",
            _ => ""
        };
    }
}
