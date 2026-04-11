using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class EmotionDetector
{
    private float smileThreshold = 0.4f;
    private float angryThreshold = 0.4f;
    private float sadThreshold = 0.4f;
    private float surprisedThreshold = 0.4f;
    private string strongestEmotion = "Neutral";
    private float strongestValue = 0f;
    private readonly int requiredStableFrames = 5;
    private Emotion lastDetectedEmotion = Emotion.Neutral;
    private int stableFrames = 0;
    public Emotion ClassifyEmotion(Dictionary<string, float> emotionScores)
    {
        strongestValue = 0;
        strongestEmotion = "Neutral";
        Emotion newEmotion = MaxEmotion(emotionScores);

        if(!CheckThreshold(newEmotion, strongestValue))
        {
           newEmotion = Emotion.Neutral;
        }
        bool isStable = TestFrameStability(newEmotion);

        if(isStable)
        {
            return newEmotion;
        }
        return Emotion.Neutral;
    }

    private Emotion MaxEmotion(Dictionary<string, float> emotionScores)
    {
        foreach (var blendshape in emotionScores)
        {
            if (blendshape.Value > strongestValue)
            {
                strongestValue = blendshape.Value;
                strongestEmotion = blendshape.Key;
            }
        }
        return EmotionMapping(strongestEmotion);
    }

    private Emotion EmotionMapping(string emotion)
    {
        return emotion switch
        {
            "Neutral" => Emotion.Neutral,
            "Smile" => Emotion.Happy,
            "Angry" => Emotion.Angry,
            "Sad" => Emotion.Sad,
            "Surprised" => Emotion.Surprised,
            _ => Emotion.Neutral
        };
    }
    private bool CheckThreshold(Emotion emotion, float value)
    {
        return emotion switch
        {
            Emotion.Happy => value > smileThreshold,
            Emotion.Angry => value > angryThreshold,
            Emotion.Sad => value > sadThreshold,
            Emotion.Surprised => value > surprisedThreshold,
            _ => false

        };
    }
    private bool TestFrameStability(Emotion newEmotion)
    {
        if (newEmotion == lastDetectedEmotion)
        {
            stableFrames++;
        }
        else
        {
            stableFrames = 0;
            lastDetectedEmotion = newEmotion;
        }
        return stableFrames >= requiredStableFrames;


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
