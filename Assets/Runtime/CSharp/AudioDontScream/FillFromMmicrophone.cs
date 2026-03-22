using UnityEngine;
using UnityEngine.UI;   
public class FillFromMmicrophone : MonoBehaviour
{
    public Image audioBar;
    public Slider sensitivitySlider;
    public AudioLoudnessDetector detector;

    public float minimumSensibility = 100;
    public float maximumSensibility = 1000;
    public float currentLoudnessSensibility = 500;
    public float threshold = 0.1f;

    public GameObject screamText;
    void Start()
    {
        if(sensitivitySlider == null) return;
        sensitivitySlider.value = .5f;
        SetLoudnessSensitivity(sensitivitySlider.value);
    }

    private void Update()
    {
        float loudness = detector.GetLoudnessFromMicrophone() * currentLoudnessSensibility;
        if (loudness < threshold) loudness = 0.01f;
        

        audioBar.fillAmount = loudness;
        if (loudness >.5f && !screamText.activeInHierarchy) screamText.SetActive(true);
        if(loudness <= .5f && screamText.activeInHierarchy) screamText.SetActive(false);
    }
    public void SetLoudnessSensitivity(float t)
    {
        currentLoudnessSensibility = Mathf.Lerp(minimumSensibility, maximumSensibility, t);
    }
}
