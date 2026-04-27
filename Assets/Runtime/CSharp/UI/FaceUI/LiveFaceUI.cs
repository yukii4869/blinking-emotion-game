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

     public void UpdateEmotion()
     {
          emotionText.text = "Emotion: " + FaceInputManager.Instance.currentEmotion;
          if (FaceInputManager.Instance.currentEmotion == Emotion.Happy)
          {
               emotionImage.texture = textures[0];
          }
          else if (FaceInputManager.Instance.currentEmotion == Emotion.Sad)
          {
               emotionImage.texture = textures[1];
          }
          else if (FaceInputManager.Instance.currentEmotion == Emotion.Angry)
          {
               emotionImage.texture = textures[2];
          }
          else if (FaceInputManager.Instance.currentEmotion == Emotion.Surprised)
          {
               emotionImage.texture = textures[3];
          }
          else if (FaceInputManager.Instance.currentEmotion == Emotion.Neutral)
          {
               emotionImage.texture = textures[4];
          }
     }
     public void UpdateBlinkCount()
     {
          blinkCounterText.text = "Blinks: " + FaceInputManager.Instance.blinkCount;
     }
}
