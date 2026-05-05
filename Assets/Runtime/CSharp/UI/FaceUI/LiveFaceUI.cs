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


     private Emotion GetCurrentEmotion()
     {
          if (CalibrationFaceInput.Instance != null)
               return CalibrationFaceInput.Instance.currentEmotion;

          if (GameplayFaceInput.Instance != null)
               return GameplayFaceInput.Instance.currentEmotion;

          return Emotion.Neutral;
     }

     private int GetBlinkCount()
     {
          if (CalibrationFaceInput.Instance != null)
               return CalibrationFaceInput.Instance.blinkCount;

          if (GameplayFaceInput.Instance != null)
               return GameplayFaceInput.Instance.blinkCount;

          return 0;
     }

     public void UpdateEmotion()
     {
          Emotion emotion = GetCurrentEmotion();
          emotionText.text = "Emotion: " + emotion;

          switch (emotion)
          {
               case Emotion.Happy: emotionImage.texture = textures[0]; break;
               case Emotion.Sad: emotionImage.texture = textures[1]; break;
               case Emotion.Angry: emotionImage.texture = textures[2]; break;
               case Emotion.Surprised: emotionImage.texture = textures[3]; break;
               default: emotionImage.texture = textures[4]; break;
          }
     }

     public void UpdateBlinkCount()
     {
          blinkCounterText.text = "Blinks: " + GetBlinkCount();
     }
}