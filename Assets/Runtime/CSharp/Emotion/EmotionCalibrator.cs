using UnityEngine;
using System.Collections.Generic;
public class EmotionCalibrator : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;

    //Dictionaries
    private Dictionary<string, float> accumulator = new();
    private Dictionary<string, float> neutralBase = new();
    private Dictionary<string, float> smileBase = new();
    private Dictionary<string, float> angryBase = new();
    private Dictionary<string, float> sadBase = new();
    private Dictionary<string, float> surprisedBase = new();
    private Dictionary<string, float> globalMax = new();
    private bool isCalibrating;

    //Finished Flags
    private bool finishedNeutral = false;
    private bool finishedSmile = false;
    private bool finishedAngry = false;
    private bool finishedSad = false;
    private bool finishedSurprised = false;
    private bool finishedAll = false;

    private int collectedFrames = 0;
    private EmotionCalibrationPhase currentPhase;
    private List<Dictionary<string, float>> finishedBaseLines = new();
    private readonly int maxFrames = 180;
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
    void Start()
    {
        foreach (string relBs in relevantBlendshapes)
        {
            globalMax[relBs] = 0f;
        }

    }
    private void Update()
    {
        if (!provider.pythonReady || !isCalibrating)
            return;

        var blendshapes = provider.blendshapes;
        if (blendshapes == null || blendshapes.Count == 0)
            return;

        ProcessCalibration(blendshapes);
    }
    private void StartCalibration(EmotionCalibrationPhase calibrationPhase)
    {
        collectedFrames = 0;
        accumulator.Clear();
        isCalibrating = true;
        currentPhase = calibrationPhase;

        //Akkumulator initial mit allen relevanten Blendshapes befüllen
        foreach (string relBs in relevantBlendshapes)
        {
            accumulator[relBs] = 0f;
        }
    }
    private void ProcessCalibration(Dictionary<string, float> blendshapes)
    {
        // for jeden blendhape aus provider.blendshape vergleiche ob in relevantBlendshape (oder andersherum) und schreibe wert auf wenn er noch nicht da ist, wenn er da ist dann addiere auf
        foreach (string relBs in relevantBlendshapes)
        {
            float currentValue = blendshapes.GetValueOrDefault(relBs, 0f);
            accumulator[relBs] += currentValue;
        }
        collectedFrames++;
        if (collectedFrames >= maxFrames)
        {
            isCalibrating = false;
            FinishCalibration(GetDictionary(currentPhase));
        }
    }
    private void FinishCalibration(Dictionary<string, float> currentCalibrationbase)
    {        
        foreach (var blendshape in accumulator) //für jeden Eintrag in accumulator (<leftMouth, 0.8>, <rightMouth, 0.9>...) durch Frame Anzahl teilen
        {
            currentCalibrationbase[blendshape.Key] = accumulator[blendshape.Key] / collectedFrames;//schreibe in das normalBase Dictionary die durchschnittlichen Werte von dem gezählten Frames
        }
        currentPhase = EmotionCalibrationPhase.None;
        
        if (currentCalibrationbase != neutralBase)
        {
            finishedBaseLines.Add(currentCalibrationbase);//Alle außer Neutral sollen geadded werden damit später global Max ausgerechent werden kann
        }

    }
    private void ComputeGlobalMax(List<Dictionary<string, float>> baseLines)
    {
        foreach (Dictionary<string, float> baseDict in baseLines)
        {
            foreach (var currBlendshape in baseDict)
            {
                globalMax[currBlendshape.Key] = Mathf.Max(currBlendshape.Value, globalMax[currBlendshape.Key]);
            }
        }
    }

    private Dictionary<string, float> GetDictionary(EmotionCalibrationPhase currentPhase)
    {
        return currentPhase switch
        {
            EmotionCalibrationPhase.Neutral => neutralBase,
            EmotionCalibrationPhase.SmileMax => smileBase,
            EmotionCalibrationPhase.AngryMax => angryBase,
            EmotionCalibrationPhase.SadMax => sadBase,
            EmotionCalibrationPhase.SurprisedMax => surprisedBase,
            _ => null,
        };
    }

    //Methoden für die Buttons oder für die UI die man einzeln triggern kann
    public void StartNeutralCalibration()
    {
        StartCalibration(EmotionCalibrationPhase.Neutral);
    }
    public void StartSmileCalibration()
    {
        StartCalibration(EmotionCalibrationPhase.SmileMax);
    }
    public void StartAngryCalibration()
    {
        StartCalibration(EmotionCalibrationPhase.AngryMax);
    }
    public void StartSadCalibration()
    {
        StartCalibration(EmotionCalibrationPhase.SadMax);
    }
    public void StartSurprisedCalibration()
    {
        StartCalibration(EmotionCalibrationPhase.SurprisedMax);
    }
    public void StartComputeGlobalMax()
    {
        ComputeGlobalMax(finishedBaseLines);
    }
}

