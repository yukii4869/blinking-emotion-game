using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

        var profile = saver.SaveProfile(nameField.text);
        ActiveProfile.Instance.SetProfile(profile);
        Hide();
        GameSceneManager.Instance.LoadGame();
    }

    public void OnCancelPressed()
    {
        Hide();
    }
}
