using UnityEngine;
public class EmotionDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private FaceInputManager face;
    [SerializeField] private EmotionHoldCondition condition;
    [SerializeField] private Animator doorAnim;


    public void Interact()
    {
        if (condition.TaskCompleted)
        {
            return;
        }
        condition.ActivateCondition();


    }
}