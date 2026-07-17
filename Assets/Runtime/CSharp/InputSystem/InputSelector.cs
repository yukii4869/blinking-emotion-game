using UnityEngine;
public enum InputMode { Face, Keyboard }
public class InputSelector : MonoBehaviour
{
    public static InputSelector Instance { get; private set; }

    public InputBase ActiveInput { get; private set; }

    public GameplayFaceInput faceInput;
    public KeyboardEmotionInput keyboardInput;

    public InputMode mode;

    private void Awake()
    {
        Instance = this;
        SwitchMode(mode);
    }

    public void SwitchMode(InputMode newMode)
    {
        mode = newMode;

        faceInput.enabled = (mode == InputMode.Face);
        keyboardInput.enabled = (mode == InputMode.Keyboard);

        ActiveInput = (mode == InputMode.Face)
            ? faceInput
            : keyboardInput;
    }

}