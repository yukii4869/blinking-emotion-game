using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool isOccupied = false;
    public int roomNumber;
    public TextMeshProUGUI roomLabel;

    private void Awake()
    {
        if (roomLabel == null)
            roomLabel = GetComponentInChildren<TextMeshProUGUI>();

        roomLabel.text = roomNumber.ToString();
    }
}
