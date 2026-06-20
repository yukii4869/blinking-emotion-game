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
        interactionHintText.text = "[Space] Bestätigungsknopf [R] Wiederholungsknopf";
        commentText.text = "Aufnahme bereit. Bitte bestätigen";
    }

    private void OnStartCalibration(InputAction.CallbackContext context)
    {
        if (waitingForContinue)
        {
            waitingForContinue = false;

            CalibrationStateManager.Instance.SetState(
                CalibrationState.EmotionCalibration);

            return;
        }

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

        statusText.text = "Bitte schauen Sie in die Kamera.";
        yield return new WaitForSeconds(2f);

        statusText.text = "Augenprofil wird erstellt.\nBitte nicht wegsehen.";

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
        statusText.text = "Augenprofil gespeichert.\n Bitte bestätigen"; ;
        waitingForContinue = true;
        calibrationRunning = false;
    }

    public void StartEARCalibration()
    {
        if (calibrationRunning || earCalibrator.finishedCalibration)
            return;

        StartCoroutine(RunEARCalibrationSequence());
    }

    public void FinishedEARCalibration()
    {
        // Nicht mehr direkt technischen Text anzeigen,
        // sonst wirkt es wieder wie Debug-UI.
        Debug.Log($"EAR gespeichert. Baseline: {earCalibrator.neutralEAR:F3}, Threshold: {earCalibrator.blinkThreshold:F3}");
    }
}