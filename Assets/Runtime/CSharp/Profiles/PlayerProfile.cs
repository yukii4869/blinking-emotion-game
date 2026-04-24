[System.Serializable]
public class PlayerProfile
{
    public string playerName;

    // EAR
    public float neutralEAR;
    public float blinkThreshold;

    // Emotion
    public SerializableDictionary<string, float> neutralBase;
    public SerializableDictionary<string, float> globalMax;
}