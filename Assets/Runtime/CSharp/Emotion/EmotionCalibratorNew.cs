using UnityEngine;
using System.Collections.Generic;

public class EmotionCalibratorNew : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;
    private bool isCalibrating;
    private Dictionary<string, float> accumulator;
    private Dictionary<string, float> neutralBase = new();
    private Dictionary<string, float> smileBase = new();
    private Dictionary<string, float> angryBase = new();
    private Dictionary<string, float> sadBase = new();
    private Dictionary<string, float> surprisedBase = new();
    private int collectedFrames = 0;
    private int currentEmotionCalibrationPhase;

    private static readonly string[] relevantBlendshapes =
{
    "browDown_L", "browDown_R",
    "browInnerUp",
    "browOuterUp_L", "browOuterUp_R",
    "cheekSquint_L", "cheekSquint_R",
    "eyeSquint_L", "eyeSquint_R",
    "eyeWide_L", "eyeWide_R",
    "jawOpen",
    "mouthSmile_L", "mouthSmile_R",
    "mouthFrown_L", "mouthFrown_R",
    "mouthPress_L", "mouthPress_R",
    "mouthShrugLower",
    "mouthUpperUp_L", "mouthUpperUp_R",
    "noseSneer_L", "noseSneer_R"
    };

    private void Update()
    {
        if (!provider.pythonReady || !isCalibrating)
            return;

        var blendshapes = provider.Blendshapes;
        if (blendshapes == null || blendshapes.Count == 0)
            return;

        //ProcessFrame(blendshapes);
    }
    private void StartCalibration()
    {
        collectedFrames = 0;
        accumulator.Clear();

    }
    private void FinishCalibration()
    {
        //für jeden Eintrag in accumulator (<leftMouth, 0.8>, <rightMouth, 0.9>...) durch Frame Anzahl teilen
        foreach (var blendshape in accumulator)
        {
            //Neues Dictionary beschreiben mit den Werten, kommt auf die Phase an
            switch (currentEmotionCalibrationPhase)
            {
                case 1:
                //schreibe in das normalBase Dictionary die durchschnittlichen Werte von dem gezählten Frames
                    break;

                case 2:
                //schreibe in das SmileBase Dictionary die Werte aus dem Akkumulator
                default:
                break;

            }
            
        }
    }
    private void CollectValues()
    {
      


    }

    // Für jede Phase immer alle aus relevantBlendshapes speichern und am Ende Max Wert nehmen
}