using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIOverlay : MonoBehaviour
{
     [SerializeField]
     private EmotionManager emotionManager;
     [SerializeField]
     private BlinkDetector blinkDetector;
     [SerializeField]
     private TextMeshProUGUI emotionText;
     [SerializeField]
     private TextMeshProUGUI blinkCounterText;

     [SerializeField]
     private RawImage emotionImage;

     [SerializeField]
     private List<Texture> textures;

     void Update()
     {
          // Emotion anzeigen
          
          emotionText.text = "Emotion: " + emotionManager.currentEmotion;
          if (emotionManager.currentEmotion == Emotion.Happy)
          {
               emotionImage.texture = textures[0];
          }
          else if (emotionManager.currentEmotion == Emotion.Sad)
          {
               emotionImage.texture = textures[1];
          }
          else if (emotionManager.currentEmotion == Emotion.Angry)
          {
               emotionImage.texture = textures[2];
          }
          else if (emotionManager.currentEmotion == Emotion.Surprised)
          {
               emotionImage.texture = textures[3];
          }
          else if (emotionManager.currentEmotion == Emotion.Neutral)
          {
               emotionImage.texture = textures[4];
          }

          blinkCounterText.text = "Blinks: " + blinkDetector.counterBlinking;




     }
}
