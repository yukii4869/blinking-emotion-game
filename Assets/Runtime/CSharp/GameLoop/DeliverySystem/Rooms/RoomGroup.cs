using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomGroup: MonoBehaviour
{
    public Transform root;
    public int startNumber;
    public List<Room> rooms = new List<Room>();
}