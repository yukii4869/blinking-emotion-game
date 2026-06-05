using UnityEngine;

public class GameplayFaceInput : MonoBehaviour
{
    public Emotion currentEmotion;
    public float currentEAR;
    public bool isBlinking;
    public int blinkCount;
    public float blinkThreshold;

    private PlayerProfile profile;
    private EARCalculator earCalc = new();
    private EmotionFeatureCalculator emotionFeatureCalc = new();
    private EmotionDetector emotionDetector = new();
    private BlendshapeNormalizer normalizer;
    private BlinkDetector blinkDetector;
    // Events
    public static event System.Action<Emotion> OnEmotionChanged;
    public static event System.Action OnBlink;
    public static event System.Action OnEyesClosed;
    private Emotion lastEmotion = Emotion.Neutral;
    public bool eyesClosed;
    private float eyesClosedTimer = 0f;

   
    public float requiredClosedDuration = 2f;


    public static GameplayFaceInput Instance { get; private set; }

    private bool ready;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
            return;
        if (!ready)
        {
            InitializeTools();
            ready = true;
        }
        ProcessBlinkDetection();
        ProcessEmotionDetection();
    }

    private void InitializeTools()
    {
        profile = ActiveProfile.Instance.CurrentProfile;

        blinkDetector = new BlinkDetector(profile.blinkThreshold);
        normalizer = new BlendshapeNormalizer(
            profile.neutralBase,
            profile.globalMax
        );
        blinkThreshold = profile.blinkThreshold;
    }

    private void ProcessBlinkDetection()
    {
        currentEAR = earCalc.ComputeBothEyes(MediaPipeProvider.Instance.Landmarks);

        blinkDetector.UpdateEAR(currentEAR);

        if (blinkDetector.BlinkStartedThisFrame)
        {
            blinkCount++;
            OnBlink?.Invoke();
        }

        isBlinking = blinkDetector.IsBlinking;

        // --- NEU: Augen wirklich geschlossen halten ---
        if (currentEAR < blinkThreshold)
        {
            eyesClosedTimer += Time.deltaTime;

            if (!eyesClosed && eyesClosedTimer >= requiredClosedDuration)
            {
                eyesClosed = true;
                OnEyesClosed?.Invoke(); // Event für GambleGuest
            }
        }
        else
        {
            eyesClosedTimer = 0f;
            eyesClosed = false;
        }

    }
    private void ProcessEmotionDetection()
    {
        var raw = MediaPipeProvider.Instance.Blendshapes;
        var norm = normalizer.NormalizeBlendshapes(raw);
        var scores = emotionFeatureCalc.CalculateEmotionFeatures(norm);
        currentEmotion = emotionDetector.ClassifyEmotion(scores);
        if (currentEmotion != lastEmotion)
        {
            OnEmotionChanged?.Invoke(currentEmotion);
            lastEmotion = currentEmotion;
        }
    }
}
