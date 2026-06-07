using UnityEngine;

public interface INoiseListener
{
    void OnNoiseHeard(Vector3 noisePos, float volume);
}