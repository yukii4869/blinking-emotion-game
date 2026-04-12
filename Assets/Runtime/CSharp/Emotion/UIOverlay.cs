using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIOverlay : MonoBehaviour
{
     [SerializeField]
     private FaceInputManager faceInputManager;
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
          
          emotionText.text = "Emotion: " + faceInputManager.currentEmotion;
          if (faceInputManager.currentEmotion == Emotion.Happy)
          {
               emotionImage.texture = textures[0];
          }
          else if (faceInputManager.currentEmotion == Emotion.Sad)
          {
               emotionImage.texture = textures[1];
          }
          else if (faceInputManager.currentEmotion == Emotion.Angry)
          {
               emotionImage.texture = textures[2];
          }
          else if (faceInputManager.currentEmotion == Emotion.Surprised)
          {
               emotionImage.texture = textures[3];
          }
          else if (faceInputManager.currentEmotion == Emotion.Neutral)
          {
               emotionImage.texture = textures[4];
          }

          blinkCounterText.text = "Blinks: " + blinkDetector.counterBlinking;




     }
}
