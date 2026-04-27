using UnityEngine;
using TMPro;

public class SaveProfileUI : MonoBehaviour
{
    [SerializeField] private CalibrationProfileSaver saver;
    [SerializeField] private TMP_InputField nameField;

    public void Show()
    {
        nameField.text = "";
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnConfirmPressed()
    {
        if (string.IsNullOrWhiteSpace(nameField.text))
            return;

        saver.SaveProfile(nameField.text);
        Hide();
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    public void OnCancelPressed()
    {
        Hide();
    }
}
