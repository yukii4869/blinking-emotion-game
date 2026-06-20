using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerLight : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Look Settings")]
    public float mouseSensitivity = 1.5f;

    [Header("Sitting Look Limits")]
    public float maxYawAngle = 20f;      // links/rechts
    public float maxPitchUp = 25f;       // nach oben
    public float maxPitchDown = 20f;     // nach unten

    [Header("Smoothing")]
    public float lookSmoothSpeed = 12f;

    private Vector2 lookInput;

    private float targetYaw;
    private float targetPitch;

    private float currentYaw;
    private float currentPitch;

    private Quaternion startBodyRotation;

    private void Start()
    {
        startBodyRotation = transform.rotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLimitedCamera();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void HandleLimitedCamera()
    {
        targetYaw += lookInput.x * mouseSensitivity;
        targetYaw = Mathf.Clamp(targetYaw, -maxYawAngle, maxYawAngle);

        targetPitch -= lookInput.y * mouseSensitivity;
        targetPitch = Mathf.Clamp(targetPitch, -maxPitchDown, maxPitchUp);

        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * lookSmoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * lookSmoothSpeed);

        transform.rotation = startBodyRotation * Quaternion.Euler(0f, currentYaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }
}