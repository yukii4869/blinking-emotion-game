using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class EARCalibrationUI : MonoBehaviour
{
    [SerializeField] EARCalibrator earCalibrator;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private InputActionReference startCalibrationAction;
    [SerializeField] private TextMeshProUGUI interactionHintText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Typewriter typewriter;
    [SerializeField] private TextMeshProUGUI commentText;

    [SerializeField] private float visibleScanDuration = 2f;
    private bool skipRequested = false;
    private bool isTyping = false;
    private bool waitingForContinue = false;
    private bool calibrationRunning = false;

    private void OnEnable()
    {
        startCalibrationAction.action.Enable();
        startCalibrationAction.action.performed += OnStartCalibration;
    }

    private void OnDisable()
    {
        startCalibrationAction.action.performed -= OnStartCalibration;
        startCalibrationAction.action.Disable();
    }

    private void Start()
    {
        earCalibrator.OnEARCalibrationFinished += FinishedEARCalibration;

        statusText.text =
            "Willkommen\n\n" +
            "Mitarbeiterprofil nicht gefunden\n\n" +
            "Registrierung erforderlich";

        progressBar.value = 0f;
        progressBar.gameObject.SetActive(false);

        interactionHintText.text = "[E] Bestätigen";
        commentText.text = "Aufnahme bereit. Bitte bestätigen";
    }

    private void OnStartCalibration(InputAction.CallbackContext context)
    {
        // BLOCKIEREN, wenn Typewriter noch schreibt
        if (isTyping)
            return;

        commentText.text = "";

        // Weiter nach Abschluss
        if (waitingForContinue)
        {
            waitingForContinue = false;

            CalibrationStateManager.Instance.SetState(
                CalibrationState.EmotionCalibration);

            return;
        }

        // Blockieren, wenn schon läuft
        if (calibrationRunning || earCalibrator.finishedCalibration)
            return;

        StartCoroutine(RunEARCalibrationSequence());
    }

    private IEnumerator RunEARCalibrationSequence()
    {
        calibrationRunning = true;
        interactionHintText.text = "";
        progressBar.value = 0f;
        progressBar.gameObject.SetActive(false);

        // TYPEWRITER: Erfassung läuft
        yield return StartCoroutine(RunTypewriter("Bitte schauen Sie in die Kamera"));

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(RunTypewriter("Erfassung läuft..."));

        progressBar.gameObject.SetActive(true);
        progressBar.value = 0f;

        earCalibrator.StartEARCalibration();

        float timer = 0f;

        while (timer < visibleScanDuration)
        {
            timer += Time.deltaTime;
            progressBar.value = Mathf.Clamp01(timer / visibleScanDuration);
            yield return null;
        }

        progressBar.value = 1f;

        yield return new WaitUntil(() => earCalibrator.finishedCalibration);

        progressBar.gameObject.SetActive(false);

        // TYPEWRITER: gespeichert
        yield return StartCoroutine(RunTypewriter("Augenprofil gespeichert."));

        commentText.text = "Bitte bestätigen";
        interactionHintText.text = "[E] Bestätigen";
        waitingForContinue = true;
        calibrationRunning = false;
    }

    public void FinishedEARCalibration()
    {
        commentText.text = "";
        Debug.Log($"EAR gespeichert. Baseline: {earCalibrator.neutralEAR:F3}, Threshold: {earCalibrator.blinkThreshold:F3}");
    }

    private IEnumerator RunTypewriter(string text)
    {
        isTyping = true;
        skipRequested = false;

        // Wir starten den Typewriter, aber kontrollieren ihn selbst
        string full = text;
        statusText.text = "";

        float delay = 1f / typewriter.charsPerSecond;

        foreach (char c in full)
        {
            // Wenn Skip gedrückt → sofort Text fertig anzeigen
            if (skipRequested)
            {
                statusText.text = full;
                break;
            }

            statusText.text += c;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;

    }
}
