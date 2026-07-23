using UnityEngine;

public class InputSelector : MonoBehaviour
{
    public static InputSelector Instance { get; private set; }

    public InputBase ActiveInput { get; private set; }

    [SerializeField] private GameplayFaceInput faceInput;
    [SerializeField] private KeyboardEmotionInput keyboardInput;
    [SerializeField] private GameObject eyelids;
    [SerializeField] private GameObject autoBlink;

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
                eyelids.SetActive(true);
                autoBlink.SetActive(false);
                break;

            case GameMode.Keyboard:
                SetKeyboardInput();
                eyelids.SetActive(true);
                autoBlink.SetActive(true);
                break;

            case GameMode.FaceNoFeedback:
                SetFaceInput();
                eyelids.SetActive(false);
                autoBlink.SetActive(false);
                break;

            case GameMode.FaceNoCalibration:
                SetFaceInput();
                eyelids.SetActive(true);
                autoBlink.SetActive(false);
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
