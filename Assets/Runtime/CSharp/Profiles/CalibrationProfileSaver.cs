using UnityEngine;

public class CalibrationProfileSaver : MonoBehaviour
{
    [SerializeField] private EARCalibrator earCalibrator;
    [SerializeField] private EmotionCalibrator emotionCalibrator;

    [SerializeField] private string currentPlayerName;

    private void Start()
    {
        emotionCalibrator.OnEmotionCalibrationFinished += SaveCalibrationProfile;
    }

    private void SaveCalibrationProfile()
    {
        PlayerProfile profile = new PlayerProfile();
        profile.playerName = currentPlayerName;

        earCalibrator.WriteEARToProfile(profile);
        emotionCalibrator.WriteEmotionToProfile(profile);

        ProfileManager.SaveProfile(profile);
    }
}
