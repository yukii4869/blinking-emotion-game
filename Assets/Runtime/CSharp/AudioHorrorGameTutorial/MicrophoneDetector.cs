using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Data;

public class MicrophoneDetector : MonoBehaviour
{
    [SerializeField] private Image pegelBalken;
    [SerializeField] private Slider sensitivitySlider;

    [SerializeField] private  float sensitivity = 1;
    [SerializeField] private int sampleSize = 246;
    [SerializeField] private float triggerThreshold = 0.5f;

    [SerializeField] private UnityEvent<float> onVolumeChange;

    private AudioClip _micClip;
    private string _micName;
    private float[] _samples;

    void Start()
    {
        if(Microphone.devices.Length == 0)
        {
            Debug.LogWarning("Es wurde kein Mikrofon gefunden");
            enabled = false;
            return;
        }
        
        _micName = Microphone.devices[0]; // Selection an der Stelle einbauen?
        _micClip = Microphone.Start(_micName, true, 10, 16000);

        _samples = new float[sampleSize];
    }
    void Update()
    {
        float volume = GetVolume01();

        if(volume > triggerThreshold)
        {
            onVolumeChange?.Invoke(volume);
        }
        pegelBalken.fillAmount = volume;
        pegelBalken.color = Color.Lerp(Color.green, Color.red, volume);
    }
    
    public void ChangeSensitivitySliderEvent()
    {
        sensitivity = sensitivitySlider.value;
    }

    public float GetVolume01()
    {
        int micPosition = Microphone.GetPosition(_micName) - sampleSize;
        if(micPosition < 0) return 0f; 

        _micClip.GetData(_samples, micPosition);

        float level = 0f;
        for(int i = 0; i < sampleSize; i++)
        {
            level += Mathf.Abs(_samples[i]);
        }
        level /= sampleSize;
        level *= sensitivity;
        level = (float)System.Math.Round((double)level, 1);
        level = Mathf.Clamp01(level);
        return level;
    }

}

