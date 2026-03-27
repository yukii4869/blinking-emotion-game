using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class EARGraph : MonoBehaviour
{
    [SerializeField] private BlinkDetector blinkDetector;
    [SerializeField] private RawImage graphImage;
    [SerializeField] private int maxSamples = 300;
    [SerializeField] private TextMeshProUGUI[] yAxisLabels;
    private float smoothedEAR = 0f;
    [SerializeField] private float smoothing = 0.2f; // 0 = kein smoothing, 1 = sehr stark

    private List<float> samples = new List<float>();

    private Texture2D texture;
    private bool visible = false;

    private InputAction toggleAction;

    void Awake()
    {
        // Taste G als InputAction registrieren
        toggleAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/g");
    }

    void OnEnable()
    {
        toggleAction.Enable();
        toggleAction.performed += ctx => ToggleGraph();
    }

    void OnDisable()
    {
        toggleAction.Disable();
    }

    void Start()
    {
        texture = new Texture2D(400, 200);
        graphImage.texture = texture;
        graphImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!visible) return;

        float ear = blinkDetector.CurrentEAR;
        smoothedEAR = Mathf.Lerp(smoothedEAR, ear, smoothing);
        ear = smoothedEAR;

        // EAR muss gültig sein
        if (float.IsNaN(ear) || ear <= 0f || ear > 1f)
            return;

        samples.Add(ear);
        if (samples.Count > maxSamples)
            samples.RemoveAt(0);

        DrawGraph();
    }

    void ToggleGraph()
    {
        visible = !visible;
        graphImage.gameObject.SetActive(visible);
    }

    void DrawGraph()
    {
        texture.Clear(Color.black);

        float scale = 400f;

        for (int i = 1; i < samples.Count; i++)
        {
            float v0 = Mathf.Clamp(samples[i - 1], 0f, 1f);
            float v1 = Mathf.Clamp(samples[i], 0f, 1f);

            int x0 = i - 1;
            int x1 = i;

            int y0 = Mathf.Clamp((int)(v0 * scale), 0, 199);
            int y1 = Mathf.Clamp((int)(v1 * scale), 0, 199);

            texture.DrawLine(x0, y0, x1, y1, Color.green);
        }

        texture.Apply();
    }


    private void UpdateYAxisLabels(float minEAR, float maxEAR)
    {
        for (int i = 0; i < yAxisLabels.Length; i++)
        {
            float t = 1f - (i / (float)(yAxisLabels.Length - 1));
            float value = Mathf.Lerp(minEAR, maxEAR, t);
            yAxisLabels[i].text = value.ToString("F2");
        }
    }

}
