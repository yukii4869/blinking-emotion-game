using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class MediaPipeProvider : MonoBehaviour
{
    [SerializeField] private UdpReceiver receiver;

    public Landmark[] Landmarks { get; private set; }
    public  Dictionary<string, float> Blendshapes;
    public bool pythonReady;

    public bool HasValidLandmarks => Landmarks != null && Landmarks.Length >= 381;
    public bool HasValidBlendshapes => Blendshapes != null;

    private void Update()
    {
        Landmarks = receiver.latestLandmarks;
        Blendshapes = receiver.latestBlendshapes; 
        pythonReady = receiver.pythonReady;
    }
}