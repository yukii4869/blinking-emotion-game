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
    public static event System.Action OnEyesOpened;
    public static event System.Action OnEyesClosedHold;
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

        bool eyesArePhysicallyClosed = currentEAR < blinkThreshold;

        // --- Augen gehen zu ---
        if (eyesArePhysicallyClosed && !eyesClosed)
        {
            eyesClosed = true;
            eyesClosedTimer = 0f;
            OnEyesClosed?.Invoke();   // ← nur EINMAL
        }

        // --- Augen bleiben zu ---
        if (eyesArePhysicallyClosed)
        {
            eyesClosedTimer += Time.deltaTime;

            if (eyesClosedTimer >= requiredClosedDuration)
                OnEyesClosedHold?.Invoke();
        }

        // --- Augen gehen wieder auf ---
        if (!eyesArePhysicallyClosed && eyesClosed)
        {
            eyesClosed = false;
            OnEyesOpened?.Invoke();   // ← jetzt wird es ausgelöst
            eyesClosedTimer = 0f;
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
