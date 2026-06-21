using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SaveProfileUI : MonoBehaviour
{
    [SerializeField] private InputActionReference enterAction;
    [SerializeField] private CalibrationProfileSaver saver;
    [SerializeField] private TMP_InputField nameField;
    private void OnEnable()
    {
        enterAction.action.Enable();
        enterAction.action.performed += OnConfirm;
    }
    private void OnDisable()
    {
        enterAction.action.performed -= OnConfirm;
        enterAction.action.Disable();
    }
    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (!gameObject.activeSelf)
            return;

        ConfirmProfile();
    }
    private void ConfirmProfile()
    {
        if (string.IsNullOrWhiteSpace(nameField.text))
            return;

        var profile = saver.SaveProfile(nameField.text);

        ActiveProfile.Instance.SetProfile(profile);

        Hide();

        GameSceneManager.Instance.LoadGame();
    }
    public void Show()
    {
        nameField.text = "";

        gameObject.SetActive(true);

        nameField.Select();
        nameField.ActivateInputField();
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
