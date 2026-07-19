using UnityEngine;

public class FlashlightToggleTrigger : MonoBehaviour
{
    [SerializeField] private Light flashlight;   // dein Spotlight am Player

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flashlight.enabled = !flashlight.enabled; 
        }
    }
}
