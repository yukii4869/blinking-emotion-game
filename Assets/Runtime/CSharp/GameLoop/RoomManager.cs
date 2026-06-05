using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    public List<Room> rooms = new List<Room>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Alle Zimmer automatisch finden
        Room[] foundRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);

        rooms.AddRange(foundRooms);
    }

    public Room GetFreeRoom()
    {
        foreach (Room r in rooms)
        {
            if (!r.isOccupied)
            {
                r.isOccupied = true;
                return r;
            }
        }

        return null; // kein Zimmer frei
    }
}
