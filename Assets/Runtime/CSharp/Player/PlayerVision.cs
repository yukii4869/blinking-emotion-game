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
    public bool IsInView(Transform target)
    {
        Camera cam = Camera.main;
        Collider col = target.GetComponent<Collider>();
        if (col == null)
            return false;

        Bounds b = col.bounds;

        // Wir prüfen NUR die obere Hälfte des Colliders
        float topY = b.max.y;
        float midY = (b.center.y + b.max.y) * 0.5f;

        Vector3[] points =
        {
        new Vector3(b.center.x, topY, b.center.z),      // Kopf
        new Vector3(b.center.x, midY, b.center.z),      // obere Brust
        new Vector3(b.min.x, midY, b.center.z),         // linke Schulter
        new Vector3(b.max.x, midY, b.center.z),         // rechte Schulter
        new Vector3(b.center.x, midY, b.min.z),         // vorne oben
        new Vector3(b.center.x, midY, b.max.z)          // hinten oben
    };

        foreach (var p in points)
        {
            Vector3 vp = cam.WorldToViewportPoint(p);

            if (vp.z < 0.1f)
                continue;

            if (vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f)
                return true;
        }

        return false;
    }
}
