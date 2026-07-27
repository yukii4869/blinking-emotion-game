using UnityEngine;
using UnityEngine.AI;

public class MinimapPathRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform player;
    private Transform target;
    public static MinimapPathRenderer Instance;
    private NavMeshPath path;
    private bool showPath = false;

    private void Awake()
    {
        Instance = this;
        path = new NavMeshPath();

        // Sichtbarkeit maximieren
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.useWorldSpace = true;

        var mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = Color.magenta;
        line.material = mat;
    }

    public void ShowPath(bool state)
    {
        showPath = state;

        if (!state)
            line.positionCount = 0;
    }

    private void Update()
    {
        if (!showPath)
            return;

        if (target == null)
        {
            line.positionCount = 0;
            Debug.LogWarning("Kein Target gesetzt!");
            return;
        }

        // Prüfen ob Player auf NavMesh ist
        if (!NavMesh.SamplePosition(player.position, out NavMeshHit hitPlayer, 1f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Player NICHT auf NavMesh → Pfad wird nicht berechnet.");
            line.positionCount = 0;
            return;
        }

        // Prüfen ob Target auf NavMesh ist
        if (!NavMesh.SamplePosition(target.position, out NavMeshHit hitTarget, 1f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Target NICHT auf NavMesh → Pfad wird nicht berechnet.");
            line.positionCount = 0;
            return;
        }

        // Pfad berechnen
        bool success = NavMesh.CalculatePath(hitPlayer.position, hitTarget.position, NavMesh.AllAreas, path);

        Debug.Log("Pfad berechnet: " + success + " | Ecken: " + path.corners.Length);

        if (!success || path.corners.Length == 0)
        {
            line.positionCount = 0;
            Debug.LogWarning("Kein Pfad gefunden!");
            return;
        }

        // Pfad zeichnen
        line.positionCount = path.corners.Length;

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 pos = path.corners[i];
            pos.y = player.position.y + 3f; // sichtbar über Boden
            line.SetPosition(i, pos);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Neues Ziel gesetzt: " + newTarget);
    }
}
