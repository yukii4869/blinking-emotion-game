using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    [SerializeField] private string deviceName;
    [SerializeField] private int sampleRate = 44100;
    [SerializeField] private int sampleLength = 1024;

    private AudioClip micClip;
    private float[] sampleBuffer;
    
    public string DeviceName => deviceName;   // Getter für DeviceName

    void Start()
    {
        sampleBuffer = new float[sampleLength];

        if (Microphone.devices.Length > 0)
            deviceName = Microphone.devices[0];

        micClip = Microphone.Start(deviceName, true, 1, sampleRate);
    }

    public float[] GetSamples()
    {
        if (micClip == null) return null;

        int micPos = Microphone.GetPosition(deviceName);

        if (micPos < sampleLength) return null;

        micClip.GetData(sampleBuffer, micPos - sampleLength);
        return sampleBuffer;
    }
}
