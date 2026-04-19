using UnityEngine;
public class EmotionDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private FaceInputManager face;
    [SerializeField] private Animator doorAnim;

    public void Interact()
    {
         Debug.Log("Door Interaction!");
        if (face.currentEmotion == Emotion.Surprised)
        {
            Debug.Log("Door open");
            doorAnim.SetTrigger("Open");
            //OpenDoor();
        }
        else
            Debug.Log("Door requires surprise!");
    }
}