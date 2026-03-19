using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

public class UdpReceiver : MonoBehaviour
{
    UdpClient client;
    Thread thread;

    public Dictionary<string, float> blendshapes = new Dictionary<string, float>();
    public Landmark[] latestLandmarks;
    private object lockObj = new object();

    void Start()
    {
        // MediaPipe_sender wird ausgeführt
        StartPythonScript();

        client = new UdpClient(5005);
        thread = new Thread(ReceiveData);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ReceiveData()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            byte[] data = client.Receive(ref ep);
            string json = Encoding.UTF8.GetString(data);

            FaceData faceData = JsonUtility.FromJson<FaceData>(json);

            if (faceData != null)
            {
                lock (lockObj)
                {
                    // Blendshapes aktualisieren
                    if (faceData.blendshapes != null)
                    {
                        var dict = new Dictionary<string, float>();
                        foreach (var e in faceData.blendshapes)
                            dict[e.key] = e.value;

                        blendshapes = dict;
                    }

                    // Landmarks aktualisieren
                    if (faceData.landmarks != null)
                    {
                        latestLandmarks = faceData.landmarks;
                    }
                }
            }
        }
    }
    void StartPythonScript()
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "python"; // oder "python3", je nach System
        psi.Arguments = "\"C:/Unity Projekte/blinking-emotion-game/Assets/Runtime/Python/Mediapipe_sender.py\"";
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        Process.Start(psi);
    }
}

[System.Serializable]
public class Landmark
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class BlendshapeEntry
{
    public string key;
    public float value;
}
[System.Serializable]
public class FaceData
{
    public Landmark[] landmarks;
    public BlendshapeEntry[] blendshapes;
}

