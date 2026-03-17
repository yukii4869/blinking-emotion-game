using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIOverlay : MonoBehaviour
{
    [SerializeField]
    private UdpReceiver receiver;
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
        emotionText.text = "Emotion: " + emotionDetector.currentEmotion;
        if (emotionDetector.currentEmotion == "happy")
        {
            emotionImage.texture = textures[0];
        }
        else if(emotionDetector.currentEmotion == "sad")
        {
             emotionImage.texture = textures[1];
        }
        else if(emotionDetector.currentEmotion == "angry")
        {
             emotionImage.texture = textures[2];
        }
        else if(emotionDetector.currentEmotion == "surprised")
        {
             emotionImage.texture = textures[3];
        }
        else if(emotionDetector.currentEmotion == "neutral")
        {
             emotionImage.texture = textures[4];
        }

        blinkCounterText.text = "Links: " + blinkDetector.leftCount + "Rechts: " + blinkDetector.rightCount;
        

        

    }
}
