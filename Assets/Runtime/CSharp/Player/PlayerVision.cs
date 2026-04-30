using UnityEngine;

public class PlayerVision : MonoBehaviour
{

    public bool isLooking;
    public static PlayerVision Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public bool IsLookingAt(Transform target)
    {
        Camera cam = Camera.main;

        Vector3 vp = cam.WorldToViewportPoint(target.position);

        // 1) Gegner im Bild?
        if (vp.z < 0 || vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1)
        {
            isLooking = false;
            Debug.Log("Nicht in View Space");
            return false;
        }

        // 2) Winkel prüfen
        Vector3 dir = (target.position - cam.transform.position).normalized;
        float angle = Vector3.Angle(cam.transform.forward, dir);
        isLooking = (angle < 30f);
        Debug.Log("Angle nicht richtig");

        return angle < 30f;
    }
    public bool IsLookingAttest(Transform target)
{
    Camera cam = Camera.main;

    Collider col = target.GetComponent<Collider>();
    if (col == null)
        return false;

    Bounds b = col.bounds;

    // Wichtige Punkte des Colliders
    Vector3[] points =
    {
        b.center,
        b.min,
        b.max,
        new Vector3(b.min.x, b.center.y, b.center.z),
        new Vector3(b.max.x, b.center.y, b.center.z),
        new Vector3(b.center.x, b.min.y, b.center.z),
        new Vector3(b.center.x, b.max.y, b.center.z),
        new Vector3(b.center.x, b.center.y, b.min.z),
        new Vector3(b.center.x, b.center.y, b.max.z)
    };

    foreach (var p in points)
    {
        Vector3 vp = cam.WorldToViewportPoint(p);

        // Punkt hinter der Kamera → ignorieren
        if (vp.z < 0.1f)
            continue;

        // Punkt im Bild?
        if (vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f)
            return true;
    }

    return false;
}


}
