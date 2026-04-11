using System.Collections.Generic;
using UnityEngine;

public class BlendshapeNormalizer
{
    private readonly Dictionary<string, float> neutralBase = new();
    private readonly Dictionary<string, float> globalMaxBase = new();
    public BlendshapeNormalizer(IReadOnlyDictionary<string, float> neutralBase, IReadOnlyDictionary<string, float> globalMaxBase)
    {
        this.neutralBase = (Dictionary<string, float>)neutralBase;
        this.globalMaxBase = (Dictionary<string, float>)globalMaxBase;
    }

    public Dictionary<string, float> NormalizeBlendshapes(Dictionary<string, float> rawBlendshapes)
    {    
        var normalizedBlendshapes = new Dictionary<string, float>();
        
        foreach (var key in neutralBase.Keys)
        {
            float raw = rawBlendshapes[key];
            float neutral = neutralBase[key];
            float max = globalMaxBase[key];
            float value = (raw - neutral) / Mathf.Max(0.000001f, max - neutral);
            value = Mathf.Clamp01(value);
            normalizedBlendshapes[key] = value;
        }
        return normalizedBlendshapes;
    }


}