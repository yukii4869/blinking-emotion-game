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

    public Dictionary<string, float> blendshapes = new Dictionary<string, float>();
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

            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);

            if (wrapper != null && wrapper.entries != null)
            {
                var dict = new Dictionary<string, float>();
                foreach (var e in wrapper.entries)
                    dict[e.key] = e.value;

                lock (lockObj)
                {
                    blendshapes = dict;
                }
            }
        }
    }

    void Update()
    {
        if (blendshapes.Count > 0)
        {
            //Debug.Log("Blendshapes geladen: " + blendshapes.Count);
            //Debug.Log("eyeBlinkLeft: " + blendshapes["eyeBlinkLeft"]);
        }
    }
}

[System.Serializable]
public class Wrapper
{
    public List<Entry> entries;
}

[System.Serializable]
public class Entry
{
    public string key;
    public float value;
}
