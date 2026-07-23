using UnityEngine;
using UnityEngine.UI;

public class GamePlayMenu : MonoBehaviour
{
    [SerializeField] private Toggle eyelidToggle;
    [SerializeField] private GameObject eyelidFeedbackObject;

    private void Start()
    {
        // Initialer Zustand basierend auf GameMode
        GameMode mode = GlobalModeStorage.Instance.SelectedMode;

        bool initialState = true;

        switch (mode)
        {
            case GameMode.FaceNoFeedback:
                initialState = false;
                break;

            default:
                initialState = true;
                break;
        }

        eyelidToggle.isOn = initialState;
        eyelidFeedbackObject.SetActive(initialState);

        // Listener registrieren
        eyelidToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool value)
    {
        eyelidFeedbackObject.SetActive(value);
    }
}
