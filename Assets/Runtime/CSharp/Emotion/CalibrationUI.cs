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

    [Header("Settings")]
    [SerializeField] private float phaseDuration = 2f; // Sekunden pro Phase
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
            StartCoroutine(RunCalibration());
        }

    }

    private IEnumerator RunCalibration()
    {
        promptText.text = "Schau ganz neutral.";
        calibrator.StartNeutralCalibration();
        yield return StartCoroutine(WaitForPhase(EmotionCalibrationPhase.Neutral));
        yield return new WaitForSeconds(2f);

        promptText.text = "Zeig dein größtes Lächeln!";
        calibrator.StartSmileMaxCalibration();
        yield return StartCoroutine(WaitForPhase(EmotionCalibrationPhase.SmileMax));
        yield return new WaitForSeconds(2f);

        promptText.text = "Schau richtig wütend!";
        calibrator.StartAngryMaxCalibration();
        yield return StartCoroutine(WaitForPhase(EmotionCalibrationPhase.AngryMax));
        yield return new WaitForSeconds(2f);

        promptText.text = "Zeig dein traurigstes Gesicht.";
        calibrator.StartSadMaxCalibration();
        yield return StartCoroutine(WaitForPhase(EmotionCalibrationPhase.SadMax));
        yield return new WaitForSeconds(2f);

        promptText.text = "Schau überrascht!";
        calibrator.StartSurprisedMaxCalibration();
        yield return StartCoroutine(WaitForPhase(EmotionCalibrationPhase.SurprisedMax));
        yield return new WaitForSeconds(2f);

        promptText.text = "Kalibrierung abgeschlossen!";
        progressBar.value = 1f;

        yield return new WaitForSeconds(1f);
        emoteCalibrationFinished = true;
        gameObject.SetActive(false);
    }

    private IEnumerator WaitForPhase(EmotionCalibrationPhase phase)
    {
        progressBar.value = 0f;
        float t = 0f;

        while (!IsPhaseFinished(phase))
        {
            t += Time.deltaTime;
            progressBar.value = Mathf.Clamp01(t / phaseDuration);
            yield return null;
        }

    }
    private bool IsPhaseFinished(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral => calibrator.NeutralFinished,
            EmotionCalibrationPhase.SmileMax => calibrator.SmileMaxFinished,
            EmotionCalibrationPhase.AngryMax => calibrator.AngryMaxFinished,
            EmotionCalibrationPhase.SadMax => calibrator.SadMaxFinished,
            EmotionCalibrationPhase.SurprisedMax => calibrator.SurprisedMaxFinished,
            _ => false
        };
    }
}
