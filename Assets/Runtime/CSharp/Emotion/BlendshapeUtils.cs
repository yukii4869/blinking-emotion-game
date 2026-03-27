using UnityEngine;
using System.Collections.Generic;
public static class BlendshapeUtils
{
    public static float Get(Dictionary<string, float> b, string key)
    {
        float value;
        if (b.TryGetValue(key, out value))
            return value;

        return 0f;
    }
}