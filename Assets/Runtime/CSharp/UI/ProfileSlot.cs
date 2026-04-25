using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button newButton;

    public void SetupFilled(string name, System.Action onLoad, System.Action onDelete)
    {
        label.text = name;

        loadButton.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        newButton.gameObject.SetActive(false);

        loadButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();

        loadButton.onClick.AddListener(() => onLoad());
        deleteButton.onClick.AddListener(() => onDelete());
    }

    public void SetupEmpty(System.Action onNew)
    {
        label.text = "";
        loadButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        newButton.gameObject.SetActive(true);

        newButton.onClick.RemoveAllListeners();
        newButton.onClick.AddListener(() => onNew());
    }
}
