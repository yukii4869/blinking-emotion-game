using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class UdpReceiver : MonoBehaviour
{
    UdpClient client;
    Thread thread;
    public bool pythonReady = false;

    public Dictionary<string, float> latestBlendshapes = new Dictionary<string, float>();
    public Landmark[] latestLandmarks;
    private object lockObj = new object();

    void Start()
    {
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

                        latestBlendshapes = dict;
                    }

                    // Landmarks aktualisieren
                    if (faceData.landmarks != null)
                    {
                        latestLandmarks = faceData.landmarks;
                    }
                }
                if (!pythonReady)
                {
                    pythonReady = true;
                    UnityEngine.Debug.Log("Python sendet - Kalibrierung kann starten.");
                }
            }
        }
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

