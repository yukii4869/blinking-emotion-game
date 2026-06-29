using UnityEngine;

public class SecurityCameraPTZ : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Parts")]
    [SerializeField] private Transform pan;   // horizontal (Y)
    [SerializeField] private Transform tilt;  // vertical (X)

    [Header("Settings")]
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private float tiltSpeed = 5f;
    [SerializeField] private float minTilt = -30f;
    [SerializeField] private float maxTilt = 30f;

    private void Update()
    {
        if (!target) return;

        Vector3 dir = target.position - pan.position;

        // --- PAN (Y-Achse) ---
        Vector3 panDir = new Vector3(dir.x, 0f, dir.z);
        Quaternion panRot = Quaternion.LookRotation(panDir);
        pan.rotation = Quaternion.Lerp(pan.rotation, panRot, panSpeed * Time.deltaTime);

        // --- TILT (X-Achse) ---
        Vector3 localDir = tilt.InverseTransformDirection(dir);
        float tiltAngle = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;
        tiltAngle = Mathf.Clamp(tiltAngle, minTilt, maxTilt);

        Vector3 tiltEuler = tilt.localEulerAngles;
        tiltEuler.x = tiltAngle;
        tiltEuler.y = 0f;
        tiltEuler.z = 0f;
        tilt.localEulerAngles = tiltEuler;
    }
}
