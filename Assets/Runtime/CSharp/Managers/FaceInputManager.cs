using UnityEngine;
using UnityEngine.SceneManagement;

public class FaceInputManager : MonoBehaviour
{
    public FaceInputMode mode = FaceInputMode.Gameplay;
    /* Public Variablen */
    public Emotion currentEmotion = Emotion.Neutral;
    public bool blinkStarted = false;
    public bool blinkEnded = false;
    public bool isBlinking = false;
    public int blinkCount = 0;
    public float currentEAR;
    public float blinkThreshold;

    /*Erstelle alle Werkzeuge*/
    private readonly EmotionFeatureCalculator emotionFeatureCalculator = new();
    private readonly EmotionDetector emotionDetector = new();
    private BlendshapeNormalizer blendshapeNormalizer;
    private EARCalculator eARCalculator = new();
    private BlinkDetector blinkDetector;

    private PlayerProfile profile;
    [Header("Calibration References")]
    [SerializeField] private EARCalibrator earCalibrator;
    [SerializeField] private EmotionCalibrator emotionCalibrator;

    public static FaceInputManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void OnEnable()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "CalibrationScene")
            mode = FaceInputMode.Calibration;
        else
            mode = FaceInputMode.Gameplay;
    }
    private void Start()
    {
        if (mode == FaceInputMode.Gameplay)
        {
            profile = ActiveProfile.Instance.CurrentProfile;

            blinkThreshold = profile.blinkThreshold;

            blinkDetector = new BlinkDetector(profile.blinkThreshold);

            blendshapeNormalizer = new BlendshapeNormalizer(
                profile.neutralBase,
                profile.globalMax
            );
        }
    }

    private void Update()
    {
        if (mode == FaceInputMode.Calibration)
        {

            if (!earCalibrator.finishedCalibration || !emotionCalibrator.finishedCalibration)
                return;

            if (blinkDetector == null)
            {
                blinkThreshold = earCalibrator.blinkThreshold;
                blinkDetector = new BlinkDetector(blinkThreshold);
            }

            if (blendshapeNormalizer == null)
            {
                blendshapeNormalizer = new BlendshapeNormalizer(
                    emotionCalibrator.GetNeutralBase(),
                    emotionCalibrator.GetGobalMax()
                );
            }
        }
        ProcessBlinkDetection();
        ProcessEmotionDetection();
    }
    private void ProcessBlinkDetection()
    {
        currentEAR = eARCalculator.ComputeBothEyes(MediaPipeProvider.Instance.Landmarks);

        blinkDetector.UpdateEAR(currentEAR);
        if (blinkDetector.BlinkStartedThisFrame)
        {
            blinkCount++;
            blinkEnded = false;
            blinkStarted = true;
            isBlinking = true;
        }
        if (blinkDetector.BlinkEndedThisFrame)
        {
            blinkStarted = false;
            blinkEnded = true;
            isBlinking = false;
        }
    }
    private void ProcessEmotionDetection()
    {
        // 1.  Blendshapes holen
        var rawBlendshapes = MediaPipeProvider.Instance.Blendshapes;
        if (rawBlendshapes == null)
        {
            return;
        }
        // 2. Normalisieren
        var normalizedBlendshapes = blendshapeNormalizer.NormalizeBlendshapes(rawBlendshapes);

        // 3. EmotionScores berechnen
        var emotionScores = emotionFeatureCalculator.CalculateEmotionFeatures(normalizedBlendshapes);

        // 4. Finale Emotion bestimmen
        currentEmotion = emotionDetector.ClassifyEmotion(emotionScores);
    }
}
public enum FaceInputMode
{
    Calibration,
    Gameplay
}


