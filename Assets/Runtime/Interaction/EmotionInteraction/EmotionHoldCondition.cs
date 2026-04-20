using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

public class EmotionHoldCondition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FaceInputManager faceInputManager;

    [Header("Condition Settings")]
    [SerializeField] private Emotion targetEmotion;
    [SerializeField] private float preDelay = 2f;   // Vorbereitungszeit
    [SerializeField] private float holdTime = 2f;   // Haltezeit
    public bool TaskCompleted { get; private set; }

    [Header("Events")]
    public UnityEvent OnConditionCompleted;

    private float timer = 0f;
    private bool isActive = false;
    private bool inPreDelay = false;
    private bool inHoldPhase = false;

    private void Awake()
    {
        enabled = false; // läuft NICHT automatisch
    }

    public void ActivateCondition()
    {
        Debug.Log($"[Condition] Aktiviert → Ziel: {targetEmotion}, Vorlauf: {preDelay}s, Haltezeit: {holdTime}s");

        timer = 0f;
        isActive = true;
        inPreDelay = true;
        inHoldPhase = false;

        enabled = true;
    }

    private void Update()
    {
        if (!isActive)
            return;

        Emotion current = faceInputManager.currentEmotion;

        // Debug: aktuelle Emotion
        Debug.Log($"[Condition] Emotion: {current}");

        // -------------------------
        // PHASE 1: PRE-DELAY
        // -------------------------
        if (inPreDelay)
        {
            timer += Time.deltaTime;
            Debug.Log($"[Condition] Vorbereitungszeit: {timer:F2}s / {preDelay}s");

            if (timer >= preDelay)
            {
                Debug.Log("[Condition] ✔ Vorbereitungszeit vorbei → Haltephase startet");
                inPreDelay = false;
                inHoldPhase = true;
                timer = 0f;
            }

            return;
        }

        // -------------------------
        // PHASE 2: HALTEPHASE
        // -------------------------
        if (inHoldPhase)
        {
            // Emotion MUSS stimmen
            if (current != targetEmotion)
            {
                Debug.Log("[Condition] Emotion verloren in Haltephase → ABBRUCH");
                StopCondition();
                return;
            }

            timer += Time.deltaTime;
            Debug.Log($"[Condition] Haltezeit: {timer:F2}s / {holdTime}s");

            if (timer >= holdTime)
            {
                Debug.Log("[Condition] ✔ Haltezeit erfolgreich → CONDITION COMPLETED");
                OnConditionCompleted?.Invoke();
                TaskCompleted = true;
                StopCondition();
            }
        }
    }

    private void StopCondition()
    {
        Debug.Log("[Condition] Condition gestoppt");
        isActive = false;
        inPreDelay = false;
        inHoldPhase = false;
        enabled = false;
    }
}
