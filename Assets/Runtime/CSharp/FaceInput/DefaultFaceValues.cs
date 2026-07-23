using System.Collections.Generic;

public static class DefaultValues
{
    // EAR / Blink
    public const float defaultEAR = 0.4f;
    // Emotion Thresholds (falls du welche nutzt)

    public static readonly Dictionary<string, float> emotionThresholds = new()
    {
        { "Smile", 0.5f },
        { "Angry", 0.4f },
        { "Sad", 0.3f },
        { "Surprised", 0.5f }
    };

    // Default Emotion Scores
    public static readonly Dictionary<string, float> Neutral = new()
    {
        { "Smile", 0.0f },
        { "Angry", 0.0f },
        { "Sad", 0.0f },
        { "Surprised", 0.0f }
    };

    public static readonly Dictionary<string, float> Smile = new()
    {
        { "Smile", 0.5f }
    };

    public static readonly Dictionary<string, float> Angry = new()
    {
        { "Angry", 0.4f }
    };

    public static readonly Dictionary<string, float> Sad = new()
    {
        { "Sad", 0.3f }
    };

    public static readonly Dictionary<string, float> Surprised = new()
    {
        { "Surprised", 0.5f }
    };
}
