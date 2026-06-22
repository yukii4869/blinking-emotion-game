using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class CalibrationValidationUI : MonoBehaviour
{
    [SerializeField] EmotionCalibrator emotionCalibrator;
    [SerializeField] EmotionCalibrationUI emotionCalibrationUI;
    [SerializeField] private InputActionReference confirmAction;
    [SerializeField] private InputActionReference retryAction;
    [SerializeField] private TextMeshProUGUI interactionHintText;

    private void OnEnable()
    {
        interactionHintText.text = "[E] Bestätigen [R] Wiederholen";
        confirmAction.action.Enable();
        retryAction.action.Enable();

        confirmAction.action.performed += OnConfirm;
        retryAction.action.performed += OnRetry;
    }

    private void OnDisable()
    {
        confirmAction.action.performed -= OnConfirm;
        retryAction.action.performed -= OnRetry;

        confirmAction.action.Disable();
        retryAction.action.Disable();
    }
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        OnAcceptPressed();
    }

    private void OnRetry(InputAction.CallbackContext ctx)
    {
        OnRetryPressed();
    }

    public void OnRetryPressed()
    {
        CalibrationStateManager.Instance.SetState(CalibrationState.EmotionCalibration);
        // 1. Erst Calibrator resetten
        emotionCalibrator.ResetAllCalibration();

        // 2. Dann UI resetten (UI MUSS aktiv sein!)
        emotionCalibrationUI.ResetUI();

    }


    public void OnAcceptPressed()
    {
        CalibrationStateManager.Instance.SetState(CalibrationState.ProfileSave);

    }
}