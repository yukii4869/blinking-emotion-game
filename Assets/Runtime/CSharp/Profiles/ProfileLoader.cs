using UnityEngine;

public class ProfileLoader : MonoBehaviour
{
    [SerializeField] private EARCalibrator earCalibrator;
    [SerializeField] private EmotionCalibrator emotionCalibrator;

    public void ApplyProfile(PlayerProfile profile)
    {
        earCalibrator.LoadFromProfile(profile);
        emotionCalibrator.LoadFromProfile(profile);
    }
}
