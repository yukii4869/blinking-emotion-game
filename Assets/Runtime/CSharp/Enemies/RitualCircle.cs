using UnityEngine;

public class RitualCircle : MonoBehaviour
{
    public float radius = 2.5f;



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Spieler sanft zurückschieben
            Vector3 pushDir = (other.transform.position - transform.position).normalized;
            other.transform.position -= pushDir * 0.2f;
        }
    }
}