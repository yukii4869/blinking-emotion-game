using UnityEngine;

public class PlayerVision : MonoBehaviour
{
    [SerializeField] private Camera cam;
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
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);

        // Wenn Z < 0 → Objekt ist hinter der Kamera
        if (viewportPos.z < 0)
            return false;

        // Wenn X oder Y außerhalb von 0–1 → Objekt ist nicht im Bild
        if (viewportPos.x < 0 || viewportPos.x > 1)
            return false;

        if (viewportPos.y < 0 || viewportPos.y > 1)
            return false;

        return true;
    }
}
