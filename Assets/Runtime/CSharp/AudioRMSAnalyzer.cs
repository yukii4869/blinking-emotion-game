using UnityEngine;

public class AudioRMSAnalyzer : MonoBehaviour
{
    [SerializeField] private MicrophoneInput mic;
    [SerializeField] private float currentRMS;
    [SerializeField] private float noiseThreshold = 0.02f;
    [SerializeField] private bool isLoud;
    public float CurrentRMS => currentRMS;

    void Update()
    {
        float[] samples = mic.GetSamples();
        if (samples == null) return;

        currentRMS = ComputeRMS(samples);
        isLoud = currentRMS > noiseThreshold;
    }

    float ComputeRMS(float[] samples)
    {
        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
            sum += samples[i] * samples[i];

        return Mathf.Sqrt(sum / samples.Length);
    }
}
