using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class UdpReceiverService
{
    private UdpClient client;
    private Thread thread;
    private bool running = false;

    private readonly object lockObj = new object();

    public bool PythonReady { get; private set; } = false;
    public Dictionary<string, float> LatestBlendshapes { get; private set; } = new();
    public Landmark[] LatestLandmarks { get; private set; }

    public void Start(int port = 5005)
    {
        client = new UdpClient(port);
        running = true;

        thread = new Thread(ReceiveLoop);
        thread.IsBackground = true;
        thread.Start();
    }

    public void Stop()
    {
        running = false;
        client?.Close();
        thread?.Join();
    }

    private void ReceiveLoop()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

        while (running)
        {
            try
            {
                byte[] data = client.Receive(ref ep);
                string json = Encoding.UTF8.GetString(data);

                FaceData faceData = JsonUtility.FromJson<FaceData>(json);

                if (faceData != null)
                {
                    lock (lockObj)
                    {
                        if (faceData.blendshapes != null)
                        {
                            var dict = new Dictionary<string, float>();
                            foreach (var e in faceData.blendshapes)
                                dict[e.key] = e.value;

                            LatestBlendshapes = dict;
                        }

                        if (faceData.landmarks != null)
                            LatestLandmarks = faceData.landmarks;
                    }

                    if (!PythonReady)
                        PythonReady = true;
                }
            }
            catch
            {
                // Ignorieren beim Stoppen
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

