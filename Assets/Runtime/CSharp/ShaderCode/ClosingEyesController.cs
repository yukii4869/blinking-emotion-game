using UnityEngine;

public class ClosingEyesController : MonoBehaviour
{
    [SerializeField] private Material eyesMaterial;

    private const float eyesOpen = 0f;
    private const float eyesClosed = 1f;
    private const float defaultSmoothness = 0.03f;

    private readonly int eyesClosedPropertyId = Shader.PropertyToID("_BlinkAmount");
    private readonly int smoothnessPropertyId = Shader.PropertyToID("_Softness");

    private float currentValue = 0f;
    private float targetValue = 0f;
    private float smoothnessValue = defaultSmoothness;

    public float openSpeed = 10f; // Öffnen schneller machen

    private bool isOpening = false;

    private void OnEnable()
    {
        GameplayFaceInput.OnEyesClosed += HandleEyesClosed;
        GameplayFaceInput.OnEyesOpened += HandleEyesOpened;
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnEyesClosed -= HandleEyesClosed;
        GameplayFaceInput.OnEyesOpened -= HandleEyesOpened;
    }

    private void HandleEyesClosed()
    {
        // Sofort schwarz
        isOpening = false;
        currentValue = eyesClosed;
        targetValue = eyesClosed;

        smoothnessValue = 0f;

        eyesMaterial.SetFloat(eyesClosedPropertyId, currentValue);
        eyesMaterial.SetFloat(smoothnessPropertyId, smoothnessValue);
    }

    private void HandleEyesOpened()
    {
        // Jetzt animiert öffnen
        isOpening = true;
        targetValue = eyesOpen;
        smoothnessValue = defaultSmoothness;
    }

    private void Update()
    {
        if (!isOpening)
            return;

        // Smooth animation for opening
        currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * openSpeed);

        // Smoothness wieder weich machen
        if (currentValue < 0.9f)
            smoothnessValue = defaultSmoothness;

        eyesMaterial.SetFloat(eyesClosedPropertyId, currentValue);
        eyesMaterial.SetFloat(smoothnessPropertyId, smoothnessValue);
    }
}
