using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class EmotionDetector 
{
    public string CurrentEmotion;
    private float smileThreshold = 0.4f;
    private float angryThreshold = 0.4f;
    private float sadThreshold = 0.4f;
    private float surprisedThreshold = 0.4f;
    public Emotion ClassifyEmotion(Dictionary<string, float> emotionScores)
    {
        

        return Emotion.Happy;
    }

    
}
public enum Emotion
{
    Neutral,
    Happy,
    Angry,
    Sad,
    Surprised
}
