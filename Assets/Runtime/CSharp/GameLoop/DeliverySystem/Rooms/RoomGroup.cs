using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomGroup
{
    public string name;
    public Transform root;
    public int startNumber;
    public List<Room> rooms = new List<Room>();
}