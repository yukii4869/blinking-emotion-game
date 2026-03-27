using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIOverlay : MonoBehaviour
{
     [SerializeField]
     private EmotionDetector emotionDetector;
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
          emotionText.text = "Emotion: " + emotionDetector.CurrentEmotion;
          if (emotionDetector.CurrentEmotion == "happy")
          {
               emotionImage.texture = textures[0];
          }
          else if (emotionDetector.CurrentEmotion == "sad")
          {
               emotionImage.texture = textures[1];
          }
          else if (emotionDetector.CurrentEmotion == "angry")
          {
               emotionImage.texture = textures[2];
          }
          else if (emotionDetector.CurrentEmotion == "surprised")
          {
               emotionImage.texture = textures[3];
          }
          else if (emotionDetector.CurrentEmotion == "neutral")
          {
               emotionImage.texture = textures[4];
          }

          blinkCounterText.text = "Blinks: " + blinkDetector.counterBlinking;




     }
}
