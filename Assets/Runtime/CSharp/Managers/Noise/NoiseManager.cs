using System.Collections.Generic;
using UnityEngine;

public static class NoiseManager
{
    private static List<INoiseListener> listeners = new List<INoiseListener>();

    public static void Register(INoiseListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public static void Unregister(INoiseListener listener)
    {
        listeners.Remove(listener);
    }

    public static void EmitNoise(Vector3 pos, float loudness)
    {
        foreach (var l in listeners)
            l.OnNoiseHeard(pos, loudness);
    }
}
