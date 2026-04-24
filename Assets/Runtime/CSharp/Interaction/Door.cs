using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Optional Condition")]
    [SerializeField] private BaseCondition condition;

    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen)
            return;

        // FALL 1: Tür hat eine Condition
        if (condition != null)
        {
            // Tür öffnet sich automatisch, wenn Condition fertig ist
            condition.OnCompleted += Open;

            // Condition starten
            condition.ActivateCondition();
            return;
        }

        // FALL 2: Tür hat KEINE Condition → direkt öffnen
        Open();
    }

    private void Open()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (animator != null)
            animator.SetTrigger("Open");
    }
}
