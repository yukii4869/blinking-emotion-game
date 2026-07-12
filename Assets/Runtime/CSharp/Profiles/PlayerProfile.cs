[System.Serializable]
public class PlayerProfile
{
    public string playerName;

    // EAR
    public float neutralEAR;
    public float blinkThreshold;

    // Emotion Baselines
    public SerializableDictionary<string, float> neutralBase;

    // Emotion Peaks
    public SerializableDictionary<string, float> smilePeak;
    public SerializableDictionary<string, float> angryPeak;
    public SerializableDictionary<string, float> sadPeak;
    public SerializableDictionary<string, float> surprisedPeak;
    // Emotion Scores
    public SerializableDictionary<string, float> neutralScores;
    public SerializableDictionary<string, float> smileScores;
    public SerializableDictionary<string, float> angryScores;
    public SerializableDictionary<string, float> sadScores;
    public SerializableDictionary<string, float> surprisedScores;

    // Optional Meta-Feature
    public SerializableDictionary<string, float> globalMax;
}
