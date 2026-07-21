using UnityEngine;

public class GlobalModeStorage : MonoBehaviour
{
    public static GlobalModeStorage Instance;

    public GameMode SelectedMode = GameMode.FaceNormal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
public enum GameMode
{
    FaceNormal,
    Keyboard,
    FaceNoFeedback,
    FaceNoCalibration
}