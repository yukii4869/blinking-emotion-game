using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class SaveProfileUI : MonoBehaviour
{
    [SerializeField] private InputActionReference enterAction;
    [SerializeField] private CalibrationProfileSaver saver;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TextMeshProUGUI statusTxt;
    [SerializeField] private Typewriter typewriter;
    [SerializeField] private TextMeshProUGUI interactionHintText;

    private bool profileConfirmed = false;

    private void OnEnable()
    {
        enterAction.action.Enable();
        enterAction.action.performed += OnConfirm;
        Show();
    }

    private void OnDisable()
    {
        enterAction.action.performed -= OnConfirm;
        enterAction.action.Disable();
    }
    private void Update()
    {
        // ESC ignorieren, damit TMP nicht den Fokus verliert
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            return;
    }
    private void LateUpdate()
    {
        if (!profileConfirmed && !nameField.isFocused)
        {
            nameField.Select();
            nameField.ActivateInputField();
        }
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy || profileConfirmed)
            return;

        ConfirmProfile();
    }

    private void ConfirmProfile()
    {
        string playerName = nameField.text.Trim();

        if (string.IsNullOrWhiteSpace(playerName))
            return;

        profileConfirmed = true;

        var profile = saver.SaveProfile(playerName);
        ActiveProfile.Instance.SetProfile(profile);

        nameField.gameObject.SetActive(false);
        interactionHintText.text = "";

        CalibrationStateManager.Instance.SetState(CalibrationState.FinishedCalibration);
    }

    public void Show()
    {
        profileConfirmed = false;
        nameField.text = "";
        nameField.gameObject.SetActive(true);
        statusTxt.text = "";
        interactionHintText.text = "[ENTER] Bestätigen";
        StartCoroutine(FocusNextFrame());
    }

    private IEnumerator FocusNextFrame()
    {
        yield return null;
        yield return null;

        nameField.Select();
        nameField.ActivateInputField();
    }
}