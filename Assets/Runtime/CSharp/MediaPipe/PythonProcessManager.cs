using UnityEngine;
using System.Diagnostics;
using System.Linq;

public class PythonProcessManager : MonoBehaviour
{
    private Process pythonProcess;

    [SerializeField] private string pythonExe = "python";
    [SerializeField] private string scriptPath = @"C:/Unity Projekte/blinking-emotion-game/Assets/Runtime/Python/Mediapipe_sender.py";

    void Start()
    {
        // Prüfen, ob Python schon läuft
        var existing = Process.GetProcessesByName("python");

        if (existing.Length > 0)
        {
            UnityEngine.Debug.Log("Python läuft bereits – starte nicht erneut.");
            pythonProcess = existing[0];
            return;
        }

        // Python starten
        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = pythonExe;
        pythonProcess.StartInfo.Arguments = $"\"{scriptPath}\"";
        pythonProcess.StartInfo.UseShellExecute = false;
        pythonProcess.StartInfo.CreateNoWindow = true;

        pythonProcess.Start();
        UnityEngine.Debug.Log("Python gestartet.");
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            pythonProcess.Dispose();
            UnityEngine.Debug.Log("Python beendet.");
        }
    }
}
