using System.Collections.Generic;
using UnityEngine;

public class MediaPipeProvider : MonoBehaviour
{
    private UdpReceiverService receiver;

    [Header("Python Settings")]
    [SerializeField] private string pythonExe = "python";
    [SerializeField] private string scriptPath = @"C:/Unity Projekte/blinking-emotion-game/Assets/Runtime/Python/Mediapipe_sender.py";
    [SerializeField] GameStateManager gameStateManager;

    private PythonProcessService python;
    private bool activated;

    public Landmark[] Landmarks { get; private set; }
    public Dictionary<string, float> Blendshapes { get; private set; }
    public bool PythonReady { get; private set; }

    public bool HasValidLandmarks => Landmarks != null && Landmarks.Length >= 381;
    public bool HasValidBlendshapes => Blendshapes != null;

    private void Start()
    {
        // Python starten
        python = new PythonProcessService();
        python.StartPython(pythonExe, scriptPath);

        // UDP Receiver starten
        receiver = new UdpReceiverService();
        receiver.Start(5005);
    }

    private void Update()
    {
        // Daten aus dem UDP-Service holen
        Landmarks = receiver.LatestLandmarks;
        Blendshapes = receiver.LatestBlendshapes;
        PythonReady = receiver.PythonReady;
        if(PythonReady && !activated)
        {
            gameStateManager.SetState(GameState.EARCalibration);
            activated = true;
        }
    }

    private void OnDestroy()
    {
        receiver?.Stop();
        python?.StopPython();
    }
}
