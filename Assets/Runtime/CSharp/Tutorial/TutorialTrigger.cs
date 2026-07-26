using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string triggerId;   // z.B. "start", "reset", "heat"

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        TutorialManager.Instance.OnTriggerActivated(triggerId);
    }
    
}