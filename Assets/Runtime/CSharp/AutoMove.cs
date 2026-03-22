using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float speed = 100f;
    public Vector3 direction = Vector3.forward;
    public void Move()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }
}