using System.Collections;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("DelayedInit");
    }
    private IEnumerator DelayedInit()
    {
        RoomManager.instance.InitializeRooms();
        yield return null; // 1 Frame warten
        DeliveryManager.Instance.InitializeDelivery();


    }
}


