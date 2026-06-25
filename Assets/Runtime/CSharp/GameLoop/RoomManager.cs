using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    public List<Room> rooms = new List<Room>();
    public List<Room> mainHall = new List<Room>();
    public List<Room> leftWing = new List<Room>();
    public List<Room> rightWing = new List<Room>();

    [Header("Auto Numbering")]
    public int startNumber = 200; // z.B. 200, 300, 400 für Etagen

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Rooms in der Reihenfolge der Hierarchie laden
        rooms = new List<Room>(GetComponentsInChildren<Room>());

        AssignRoomNumbers();
        AssignDeliverySpots();
        UpdateRoomLabels();
    }

    private void AssignRoomNumbers()
    {
        
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].roomNumber = startNumber + i;
        }
    }

    private void AssignDeliverySpots()
    {
        foreach (var room in rooms)
        {
            DeliverySpot spot = room.GetComponentInChildren<DeliverySpot>();
            if (spot != null)
            {
                spot.room = room;
            }
        }
    }

    private void UpdateRoomLabels()
    {
        foreach (var room in rooms)
        {
            if (room.roomLabel != null)
                room.roomLabel.text = room.roomNumber.ToString();
        }
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

        return null;
    }
}
