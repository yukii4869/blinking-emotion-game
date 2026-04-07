using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class MediaPipeProvider : MonoBehaviour
{
    [SerializeField] private UdpReceiver receiver;

    public Landmark[] Landmarks { get; private set; }
    public  Dictionary<string, float> blendshapes;
    public bool pythonReady;

    public bool HasValidLandmarks => Landmarks != null && Landmarks.Length >= 381;
    public bool HasValidBlendshapes => blendshapes != null;

    private void Update()
    {
        Landmarks = receiver.latestLandmarks;
        blendshapes = receiver.latestBlendshapes; 
        pythonReady = receiver.pythonReady;
    }
}