using UnityEngine;

public class LookZoneConfirm : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject hintObject;

    [Header("Look Zone")]
    [SerializeField] private float minYaw = 15f;   // rechts
    [SerializeField] private float maxYaw = 60f;
    [SerializeField] private float minPitch = 10f; // nach unten
    [SerializeField] private float maxPitch = 45f;

    public bool IsLookingAtConfirmZone { get; private set; }

    private void Update()
    {
        Vector3 localEuler = playerCamera.localEulerAngles;

        float yaw = NormalizeAngle(localEuler.y);
        float pitch = NormalizeAngle(localEuler.x);

        bool lookingRight = yaw >= minYaw && yaw <= maxYaw;
        bool lookingDown = pitch >= minPitch && pitch <= maxPitch;

        IsLookingAtConfirmZone = lookingRight && lookingDown;

        if (hintObject != null)
            hintObject.SetActive(IsLookingAtConfirmZone);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}