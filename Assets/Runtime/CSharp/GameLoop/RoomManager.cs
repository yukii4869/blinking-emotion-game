using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public Transform mainHallRoot;
    public Transform leftWingRoot;
    public Transform rightWingRoot;



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

    public void InitializeRooms()
    {
        mainHall = new List<Room>(mainHallRoot.GetComponentsInChildren<Room>());
        leftWing = new List<Room>(leftWingRoot.GetComponentsInChildren<Room>());
        rightWing = new List<Room>(rightWingRoot.GetComponentsInChildren<Room>());
        AssignRoomNumbers();
        AssignDeliverySpots();
        UpdateRoomLabels();
    }
    private void AssignRoomNumbers()
    {
        int main = startNumber;        // z.B. 200
        int left = startNumber + 8;   // z.B. 210
        int right = startNumber + 24;  // z.B. 220

        foreach (var r in mainHall)
            r.roomNumber = main++;

        foreach (var r in leftWing)
            r.roomNumber = left++;

        foreach (var r in rightWing)
            r.roomNumber = right++;
    }

    private void AssignDeliverySpots()
    {
        foreach (var room in mainHall)
            LinkSpot(room);

        foreach (var room in leftWing)
            LinkSpot(room);

        foreach (var room in rightWing)
            LinkSpot(room);
    }

    private void LinkSpot(Room room)
    {
        DeliverySpot spot = room.GetComponentInChildren<DeliverySpot>();
        if (spot != null)
            spot.room = room;
    }

    private void UpdateRoomLabels()
    {
        foreach (var room in mainHall)
            UpdateLabel(room);

        foreach (var room in leftWing)
            UpdateLabel(room);

        foreach (var room in rightWing)
            UpdateLabel(room);
    }

    private void UpdateLabel(Room room)
    {
        if (room.roomLabel != null)
            room.roomLabel.text = room.roomNumber.ToString();
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
