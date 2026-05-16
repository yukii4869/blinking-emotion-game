using UnityEngine;

public class MicInputManagerGamePlay : MonoBehaviour
{
    public static MicInputManagerGamePlay Instance;

    [Header("Mic Settings")]
    public float sensitivity = 0.03f;   // Default threshold
    public float loudness;
    public float smoothSpeed = 10f;     // Wie schnell glätten?

    [Header("Noise Info")]
    public Vector3 lastNoisePosition;

    private AudioClip micClip;
    private string device;
    private const int sampleWindow = 128;
    public int currentDeviceIndex = 0;
    [SerializeField] public string currentDeviceName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentDeviceIndex = PlayerPrefs.GetInt("SelectedMicIndex", 0);
        

        if (Microphone.devices.Length > 0)
        {
            device = Microphone.devices[currentDeviceIndex];
            micClip = Microphone.Start(device, true, 1, 44100);
            currentDeviceName = device;
        }
        else
        {
            Debug.LogWarning("Kein Mikrofon gefunden!");
        }
    }


    public float GetLoudness()
    {
        if (micClip == null)
            return 0f;

        int micPos = Microphone.GetPosition(device);

        // Mikrofon liefert noch keine Daten
        if (micPos < sampleWindow || micPos <= 0)
            return 0f;

        float[] data = new float[sampleWindow];

        int startPos = micPos - sampleWindow;
        if (startPos < 0)
            return 0f;

        micClip.GetData(data, startPos);

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
            sum += Mathf.Abs(data[i]);

        return sum / sampleWindow;

    }

    public void ChangeMicrophone(int index)
    {
        // Altes Mic stoppen
        Microphone.End(device);

        // Neues Gerät setzen
        currentDeviceIndex = index;
        device = Microphone.devices[index];
        currentDeviceName = device;

        // Neues Mic starten
        micClip = Microphone.Start(device, true, 1, 44100);
    }
    private void OnEnable()
    {
        MicSettingsUI.OnMicrophoneChoiceChanged += ChangeMicrophone;
    }

    private void OnDisable()
    {
        MicSettingsUI.OnMicrophoneChoiceChanged -= ChangeMicrophone;
    }

}
