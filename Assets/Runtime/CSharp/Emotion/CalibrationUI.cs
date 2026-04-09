using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class CalibrationUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EmotionCalibrator calibrator;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private EARCalibration earCalc;
    [SerializeField] GameObject button;
    private bool phaseRunning = false;

    [Header("Settings")]
    private bool calibrationStarted = false;
    private bool emoteCalibrationFinished = false;

    private void Update()
    {
        if (!earCalc.calibrationFinished)
            return;
        if (!calibrationStarted && !emoteCalibrationFinished)
        {
            calibrationStarted = true;
            progressBar.value = 0f;
            button.SetActive(true);
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
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.Neutral) && !IsPhaseFinished(EmotionCalibrationPhase.SmileMax))
        {
            StartCoroutine(RunPhase("Zeig dein schönstes Lächeln", EmotionCalibrationPhase.SmileMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.SmileMax) && !IsPhaseFinished(EmotionCalibrationPhase.AngryMax))
        {
            StartCoroutine(RunPhase("Schau richtig wütend", EmotionCalibrationPhase.AngryMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.AngryMax) && !IsPhaseFinished(EmotionCalibrationPhase.SadMax))
        {
            StartCoroutine(RunPhase("Schau richtig traurig", EmotionCalibrationPhase.SadMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.SadMax) && !IsPhaseFinished(EmotionCalibrationPhase.SurprisedMax))
        {
            StartCoroutine(RunPhase("Schau überrascht", EmotionCalibrationPhase.SurprisedMax));
        }
        else if (IsPhaseFinished(EmotionCalibrationPhase.SurprisedMax))
        {
            calibrator.StartComputeGlobalMax();
            promptText.text = "Kalibrierung abgeschlossen!";
            progressBar.value = 1f;
            emoteCalibrationFinished = true;
            return;
        }
    }
    private IEnumerator RunPhase(string prompt, EmotionCalibrationPhase phase)
    {
        if (phaseRunning)
        {
            yield break;// verhindert Doppelstart
        }
        phaseRunning = true;
        promptText.text = prompt;

        yield return StartCoroutine(Countdown());

        switch (phase)
        {
            case EmotionCalibrationPhase.Neutral:
                calibrator.StartNeutralCalibration();
                break;
            case EmotionCalibrationPhase.SmileMax:
                calibrator.StartSmileCalibration();
                break;
            case EmotionCalibrationPhase.AngryMax:
                calibrator.StartAngryCalibration();
                break;
            case EmotionCalibrationPhase.SadMax:
                calibrator.StartSadCalibration();
                break;
            case EmotionCalibrationPhase.SurprisedMax:
                calibrator.StartSurprisedCalibration();
                break;
        }

        yield return StartCoroutine(ProgressCalibration(phase));
        phaseRunning = false;
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
            progressBar.value = (float)calibrator.collectedFrames / calibrator.maxFrames;
            yield return null;
        }
    }
    private bool IsPhaseFinished(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral => calibrator.finishedNeutral,
            EmotionCalibrationPhase.SmileMax => calibrator.finishedSmile,
            EmotionCalibrationPhase.AngryMax => calibrator.finishedAngry,
            EmotionCalibrationPhase.SadMax => calibrator.finishedSad,
            EmotionCalibrationPhase.SurprisedMax => calibrator.finishedSurprised,
            _ => false
        };
    }
}
