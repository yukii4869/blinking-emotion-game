using UnityEngine;
public class EmotionDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private FaceInputManager face;

    public void Interact()
    {
        if (face.currentEmotion == Emotion.Surprised)
            Debug.Log("Door open");
            //OpenDoor();
        else
            Debug.Log("Door requires surprise!");
    }
}