using System.Collections.Generic;
using UnityEngine;

public class MediaPipeProvider : MonoBehaviour
{
    private UdpReceiverService receiver;

    [Header("Python Settings")]
    [SerializeField] private string pythonExe = "python";
    [SerializeField] private string scriptPath = @"C:/Unity Projekte/blinking-emotion-game/Assets/Runtime/Python/Mediapipe_sender.py";

    private PythonProcessService python;
    private bool activated;

    public Landmark[] Landmarks { get; private set; }
    public Dictionary<string, float> Blendshapes { get; private set; }
    public bool PythonReady { get; private set; }

    public bool HasValidLandmarks => Landmarks != null && Landmarks.Length >= 381;
    public bool HasValidBlendshapes => Blendshapes != null;
    public static MediaPipeProvider Instance { get; private set; }
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!IsRunning)
            return;
        // Daten aus dem UDP-Service holen
        Landmarks = receiver.LatestLandmarks;
        Blendshapes = receiver.LatestBlendshapes;
        PythonReady = receiver.PythonReady;
        if (PythonReady && !activated)
        {
            if(CalibrationStateManager.Instance!= null)
            {
                CalibrationStateManager.Instance.SetState(CalibrationState.EARCalibration);
            }
            if(GameStateManager.Instance != null)
            {
                
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }
            
            activated = true;
        }
    }
    public void StartMediaPipe()
    {
        if (python == null)
            python = new PythonProcessService();

        python.StartPython(pythonExe, scriptPath);

        if (receiver == null)
            receiver = new UdpReceiverService();

        receiver.Start(5005);

        IsRunning = true;
    }

    public void StopMediaPipe()
    {
        receiver?.Stop();
        python?.StopPython();

        receiver = null;
        python = null;

        Landmarks = null;
        Blendshapes = null;
        PythonReady = false;
        activated = false;

        IsRunning = false;
    }

    public void OnDestroy()
    {
        receiver?.Stop();
        python?.StopPython();
    }
}
