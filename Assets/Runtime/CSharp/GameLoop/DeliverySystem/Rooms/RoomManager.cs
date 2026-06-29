using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [Header("Room Groups")]
    public List<RoomGroup> groups = new List<RoomGroup>();

    public List<Room> rooms = new List<Room>();

    void Awake()
    {
        instance = this;
    }

    public void InitializeRooms()
    {

        StartCoroutine(DelayedInit());
    }
    private IEnumerator DelayedInit()
    {
        yield return null; // 1 Frame warten
        Debug.Log("RoomManager.Start() wurde ausgeführt!");
        rooms.Clear();
        Debug.Log("RoomManager.Start(): groups.Count = " + groups.Count);

        foreach (var g in groups)
        {

            Debug.Log("Group: " + g.name);
            Debug.Log("Root: " + g.root);
            Debug.Log("Rooms found: " + (g.root == null ? "ROOT NULL" : g.root.GetComponentsInChildren<Room>().Length.ToString()));

            // Rooms laden
            g.rooms = new List<Room>(g.root.GetComponentsInChildren<Room>());

            // Nummern vergeben
            AssignRoomNumbers(g);

            // Delivery Spots setzen
            AssignDeliverySpots(g);

            // Labels updaten
            UpdateRoomLabels(g);

            // Globale Liste füllen
            rooms.AddRange(g.rooms);
        }
    }

    private void AssignRoomNumbers(RoomGroup g)
    {
        int number = g.startNumber;

        foreach (var r in g.rooms)
            r.roomNumber = number++;
    }

    private void AssignDeliverySpots(RoomGroup g)
    {
        foreach (var room in g.rooms)
        {
            DeliverySpot spot = room.GetComponentInChildren<DeliverySpot>();
            if (spot != null)
                spot.room = room;
        }
    }

    private void UpdateRoomLabels(RoomGroup g)
    {
        foreach (var room in g.rooms)
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
