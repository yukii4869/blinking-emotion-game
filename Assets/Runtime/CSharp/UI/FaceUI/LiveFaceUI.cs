using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LiveFaceUI : MonoBehaviour
{
    [SerializeField] private RawImage emotionImage;
    [SerializeField] private TextMeshProUGUI emotionText;
    [SerializeField] private TextMeshProUGUI blinkCounterText;
    [SerializeField] private List<Texture> textures;

    private void OnEnable()
    {
        GameplayFaceInput.OnEmotionChanged += HandleEmotion;
        GameplayFaceInput.OnBlink += HandleBlink;   // optional, siehe unten
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnEmotionChanged -= HandleEmotion;
        GameplayFaceInput.OnBlink -= HandleBlink;
    }

    private void HandleEmotion(Emotion e)
    {
        emotionText.text = "Emotion: " + e;

        switch (e)
        {
            case Emotion.Happy: emotionImage.texture = textures[0]; break;
            case Emotion.Sad: emotionImage.texture = textures[1]; break;
            case Emotion.Angry: emotionImage.texture = textures[2]; break;
            case Emotion.Surprised: emotionImage.texture = textures[3]; break;
            default: emotionImage.texture = textures[4]; break;
        }
    }

    private void HandleBlink(int blinkCount)
    {
        blinkCounterText.text = "Blinks: " + blinkCount;
    }
}
