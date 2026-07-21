using UnityEngine;

public class InputSelector : MonoBehaviour
{
    public static InputSelector Instance { get; private set; }

    public InputBase ActiveInput { get; private set; }

    [SerializeField] private GameplayFaceInput faceInput;
    [SerializeField] private KeyboardEmotionInput keyboardInput;
    [SerializeField] private GameObject Eyelids;

    private void Awake()
    {
        Instance = this;
        ApplySelectedMode();
    }
    private void ApplySelectedMode()
    {
        GameMode mode = GlobalModeStorage.Instance.SelectedMode;

        switch (mode)
        {
            case GameMode.FaceNormal:
                SetFaceInput();
                Eyelids.SetActive(true);
                break;

            case GameMode.Keyboard:
                SetKeyboardInput();
                Eyelids.SetActive(true);
                break;

            case GameMode.FaceNoFeedback:
                SetFaceInput();
                Eyelids.SetActive(false);
                break;

            case GameMode.FaceNoCalibration:
                SetFaceInput();
                Eyelids.SetActive(true);
                break;
        }
    }

    private void SetFaceInput()
    {
        faceInput.enabled = true;
        keyboardInput.enabled = false;
        ActiveInput = faceInput;
    }

    private void SetKeyboardInput()
    {
        faceInput.enabled = false;
        keyboardInput.enabled = true;
        ActiveInput = keyboardInput;
    }
}
