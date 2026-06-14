using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private EmotionCondition condition;

    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen) return;

        if (condition != null)
        {
            condition.StartCondition(Open, Fail);
            return;
        }

        Open();
    }

    private void Open()
    {
        if (isOpen) return;
        isOpen = true;

        animator.SetTrigger("Open");
    }

    private void Fail()
    {
        Debug.Log("Emotion failed!");
        // später Sound
    }
}
