using UnityEngine;

public class CalibrationUIManger : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject calibrationBackground;
    [SerializeField] private GameObject pythonLoadingUI;
    [SerializeField] private GameObject earCalibrationUI;
    [SerializeField] private GameObject emotionCalibrationUI;
    [SerializeField] private GameObject emotionTestUI;
    [SerializeField] private GameObject saveProfileUI;
    [SerializeField] private GameObject liveFaceUI;
    [SerializeField] private GameObject interactionHintUI;
    [SerializeField] private GameObject finishedCalibrationUI;

    private void Start()
    {
        CalibrationStateManager.Instance.OnStateChanged += HandleStateChanged;
        CalibrationStateManager.Instance.SetState(CalibrationState.PythonPreparation);
        HandleStateChanged(CalibrationStateManager.Instance.CurrentState);
    }
    private void HandleStateChanged(CalibrationState state)
    {
        // Alles aus
        calibrationBackground.SetActive(false);
        pythonLoadingUI.SetActive(false);
        earCalibrationUI.SetActive(false);
        emotionCalibrationUI.SetActive(false);
        emotionTestUI.SetActive(false);
        saveProfileUI.SetActive(false);
        liveFaceUI.SetActive(false);
        interactionHintUI.SetActive(true);
        finishedCalibrationUI.SetActive(false);


        // Passende UI an
        switch (state)
        {
            case CalibrationState.PythonPreparation:
                pythonLoadingUI.SetActive(true);
                calibrationBackground.SetActive(true);
                interactionHintUI.SetActive(false);
                break;
            case CalibrationState.EARCalibration:
                earCalibrationUI.SetActive(true);
                break;

            case CalibrationState.EmotionCalibration:
                earCalibrationUI.SetActive(true);
                emotionCalibrationUI.SetActive(true);
                break;

            case CalibrationState.EmotionTest:

                emotionTestUI.SetActive(true);
                liveFaceUI.SetActive(true);
                break;
            case CalibrationState.ProfileSave:

                saveProfileUI.SetActive(true);
                break;
            case CalibrationState.FinishedCalibration:
                Debug.Log("FinishedCalibrationStarted");
                finishedCalibrationUI.SetActive(true);
                break;
        }

    }
}