using UnityEngine;

public class CalibrationManger : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject calibrationBackground;
    [SerializeField] private GameObject pythonLoadingUI;
    [SerializeField] private GameObject profileLoaderUI;
    [SerializeField] private GameObject earCalibrationUI;
    [SerializeField] private GameObject emotionCalibrationUI;
    [SerializeField] private GameObject emotionTestUI;
    [SerializeField] private GameObject saveProfileUI;
    [SerializeField] private GameObject liveFaceUI;

    private void Start()
    {
        CalibrationStateManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(CalibrationStateManager.Instance.CurrentState);
    }
    private void HandleStateChanged(CalibrationState state)
    {
        // Alles aus
        calibrationBackground.SetActive(false);
        pythonLoadingUI.SetActive(false);
        profileLoaderUI.SetActive(false);
        earCalibrationUI.SetActive(false);
        emotionCalibrationUI.SetActive(false);
        emotionTestUI.SetActive(false);
        saveProfileUI.SetActive(false);
        liveFaceUI.SetActive(false);

        // Passende UI an
        switch (state)
        {
            case CalibrationState.PythonPreparation:
                pythonLoadingUI.SetActive(true);
                calibrationBackground.SetActive(true);
                break;

            case CalibrationState.ProfileSelection:
                calibrationBackground.SetActive(true);
                profileLoaderUI.SetActive(true);
                break;

            case CalibrationState.EARCalibration:
                earCalibrationUI.SetActive(true);
                calibrationBackground.SetActive(true);
                liveFaceUI.SetActive(true);
                break;

            case CalibrationState.EmotionCalibration:
                earCalibrationUI.SetActive(true);
                emotionCalibrationUI.SetActive(true);
                calibrationBackground.SetActive(true);
                liveFaceUI.SetActive(true);
                break;

            case CalibrationState.EmotionTest:
                calibrationBackground.SetActive(true);
                liveFaceUI.SetActive(true);
                emotionTestUI.SetActive(true);
                break;
            case CalibrationState.ProfileSave:
                calibrationBackground.SetActive(true);
                saveProfileUI.SetActive(true);
                break;
        }

    }
}