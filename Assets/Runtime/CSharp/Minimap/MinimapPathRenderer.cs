using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinimapPathRenderer : MonoBehaviour
{
    [Header("Dot Settings")]
    [SerializeField] private GameObject dotPrefab;      // kleines rundes Sprite/Quad
    [SerializeField] private float dotSpacing = 1.5f;    // Abstand zwischen Punkten
    [SerializeField] private float dotHeightOffset = 3.5f;
    [SerializeField] private Transform player;
    [SerializeField] private Transform dotParent;        // leeres Objekt zum Aufräumen im Hierarchy-Fenster

    [Header("Recalculation")]
    [SerializeField] private float recalcInterval = 0.2f;
    private float recalcTimer;

    private Transform target;
    private NavMeshPath path;
    private bool showPath = false;

    public static MinimapPathRenderer Instance;

    private readonly List<GameObject> dotPool = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        path = new NavMeshPath();
    }

    public void ShowPath(bool state)
    {
        showPath = state;
        if (!state)
            HideAllDots();
    }

    private void Update()
    {
        if (!showPath || target == null)
        {
            if (target == null) HideAllDots();
            return;
        }

        recalcTimer -= Time.deltaTime;
        if (recalcTimer > 0f)
            return;

        recalcTimer = recalcInterval;
        RecalculateAndDrawPath();
    }

    private void RecalculateAndDrawPath()
    {
        if (!NavMesh.SamplePosition(player.position, out NavMeshHit hitPlayer, 1f, NavMesh.AllAreas))
        {
            HideAllDots();
            return;
        }

        if (!NavMesh.SamplePosition(target.position, out NavMeshHit hitTarget, 1f, NavMesh.AllAreas))
        {
            HideAllDots();
            return;
        }

        bool success = NavMesh.CalculatePath(hitPlayer.position, hitTarget.position, NavMesh.AllAreas, path);

        if (!success || path.corners.Length < 2)
        {
            HideAllDots();
            return;
        }

        DrawDots(path.corners);
    }

    private void DrawDots(Vector3[] corners)
    {
        var points = new List<Vector3>();

        // Punkte entlang jedes Segments in festem Abstand erzeugen
        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[i + 1];
            float segmentLength = Vector3.Distance(start, end);
            int stepsOnSegment = Mathf.Max(1, Mathf.FloorToInt(segmentLength / dotSpacing));

            for (int s = 0; s < stepsOnSegment; s++)
            {
                float t = s / (float)stepsOnSegment;
                points.Add(Vector3.Lerp(start, end, t));
            }
        }
        points.Add(corners[corners.Length - 1]);

        // Pool auffüllen falls nötig
        while (dotPool.Count < points.Count)
        {
            var dot = Instantiate(dotPrefab, dotParent);
            dot.SetActive(false);
            dotPool.Add(dot);
        }

        // Punkte positionieren
        for (int i = 0; i < dotPool.Count; i++)
        {
            if (i < points.Count)
            {
                Vector3 pos = points[i];
                pos.y = player.position.y + dotHeightOffset;
                dotPool[i].transform.position = pos;
                dotPool[i].SetActive(true);
            }
            else
            {
                dotPool[i].SetActive(false);
            }
        }
    }

    private void HideAllDots()
    {
        foreach (var dot in dotPool)
            dot.SetActive(false);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}