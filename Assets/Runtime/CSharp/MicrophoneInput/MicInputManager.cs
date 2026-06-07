using UnityEngine;

public class MicInputManager : MonoBehaviour
{
    public static MicInputManager Instance;

    [Header("Mic Settings")]
    public float sensitivity = 0.03f;
    public float smoothedLoudness;
    public float smoothSpeed = 10f;

    private AudioClip micClip;
    private string device;
    private const int sampleWindow = 128;

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

    private void Start()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("Kein Mikrofon gefunden!");
            return;
        }

        device = Microphone.devices[0];
        micClip = Microphone.Start(device, true, 1, 44100);
    }

    private void Update()
    {
        float raw = GetLoudness();
        float normalized = Mathf.Clamp01(raw / sensitivity);

        smoothedLoudness = Mathf.Lerp(smoothedLoudness, normalized, Time.unscaledDeltaTime * smoothSpeed);

        // Geräusch ins NoiseSystem schicken von Spieler Mikrofon
        if (smoothedLoudness > 0.1f)
        {
            NoiseManager.EmitNoise(Camera.main.transform.position, smoothedLoudness);
        }
    }

    private float GetLoudness()
    {
        if (micClip == null) return 0f;

        int micPos = Microphone.GetPosition(device);
        if (micPos < sampleWindow) return 0f;

        float[] data = new float[sampleWindow];
        micClip.GetData(data, micPos - sampleWindow);

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
            sum += Mathf.Abs(data[i]);

        return sum / sampleWindow;
    }
}
