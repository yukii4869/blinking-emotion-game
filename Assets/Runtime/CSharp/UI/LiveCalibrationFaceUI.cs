using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LiveCalibrationFaceUI : MonoBehaviour
{
    [SerializeField] private RawImage emotionImage;
    [SerializeField] private TextMeshProUGUI emotionText;
    [SerializeField] private TextMeshProUGUI blinkCounterText;
    [SerializeField] private List<Texture> textures;

    private void Update()
    {
        if (CalibrationFaceInput.Instance == null)
            return;

        var face = CalibrationFaceInput.Instance;

        // Emotion anzeigen
        emotionText.text = "Emotion: " + face.currentEmotion;

        switch (face.currentEmotion)
        {
            case Emotion.Happy: emotionImage.texture = textures[0]; break;
            case Emotion.Sad: emotionImage.texture = textures[1]; break;
            case Emotion.Angry: emotionImage.texture = textures[2]; break;
            case Emotion.Surprised: emotionImage.texture = textures[3]; break;
            default: emotionImage.texture = textures[4]; break;
        }

        // Blink Counter
        blinkCounterText.text = "Blinks: " + face.blinkCount;
    }
}
