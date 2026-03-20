using UnityEngine;
using UnityEngine.UI;

public class AudioRMSVisualizer : MonoBehaviour
{
    [SerializeField] private AudioRMSAnalyzer analyzer;
    [SerializeField] private Image bar;
    [SerializeField] private float maxRMS = 0.2f;
    [SerializeField] private float smoothSpeed = 10f;

    private float smoothedValue = 0f;

    void Update()
    {
        float target = Mathf.Clamp01(analyzer.CurrentRMS / maxRMS);
        smoothedValue = Mathf.Lerp(smoothedValue, target, Time.deltaTime * smoothSpeed);

        bar.fillAmount = smoothedValue;

        if (smoothedValue < 0.3f)
            bar.color = Color.green;
        else if (smoothedValue < 0.6f)
            bar.color = Color.yellow;
        else
            bar.color = Color.red;
    }
}
