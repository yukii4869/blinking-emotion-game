using UnityEngine;

public class CalibrationProfileSaver : MonoBehaviour
{
    [SerializeField] private EARCalibrator earCalibrator;
    [SerializeField] private EmotionCalibrator emotionCalibrator;

    public PlayerProfile SaveProfile(string playerName)
    {
        PlayerProfile profile = new PlayerProfile();
        profile.playerName = playerName;

        earCalibrator.WriteEARToProfile(profile);
        emotionCalibrator.WriteEmotionToProfile(profile);

        ProfileManager.SaveProfile(profile);
         return profile;
    }
}
