using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [Header("Room Groups")]
    private List<RoomGroup> groups = new List<RoomGroup>();

    public List<Room> rooms = new List<Room>();

    void Awake()
    {
        instance = this;
    }
    public void InitializeRooms()
    {
        if (groups.Count == 0)
        {
            groups = new List<RoomGroup>(FindObjectsOfType<RoomGroup>());
        }

        StartCoroutine(DelayedInit());
    }
    private IEnumerator DelayedInit()
    {
        yield return null; // 1 Frame warten
        rooms.Clear();
        foreach (var g in groups)
        {
            // Rooms laden
            g.rooms = new List<Room>(g.root.GetComponentsInChildren<Room>());
            // Globale Liste füllen
            rooms.AddRange(g.rooms);
            // Nummern vergeben
            AssignRoomNumbers(g);

            // Labels updaten
            UpdateRoomLabels(g);


        }
    }
    private void AssignRoomNumbers(RoomGroup g)
    {
        int number = g.startNumber;

        foreach (var r in g.rooms)
            r.roomNumber = number++;
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
    public Room GetRoomByNumber(int number)
    {
        foreach (var r in rooms)
        {
            if (r.roomNumber == number)
                return r;
        }
        return null;
    }
}
