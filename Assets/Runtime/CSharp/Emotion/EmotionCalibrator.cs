using UnityEngine;
using System.Collections.Generic;
public class EmotionCalibrator : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;
    [SerializeField] private GameStateManager gameStateManager;
    
    //Dictionaries
    private Dictionary<string, float> accumulator = new();
    private Dictionary<string, float> neutralBase = new();
    private Dictionary<string, float> smileBase = new();
    private Dictionary<string, float> angryBase = new();
    private Dictionary<string, float> sadBase = new();
    private Dictionary<string, float> surprisedBase = new();
    private Dictionary<string, float> globalMax = new();
    private bool isCalibrating;
    private EmotionCalibrationPhase currentPhase;
    private List<Dictionary<string, float>> finishedBaseLines = new();

    private static readonly string[] relevantBlendshapes =
    {
    "browDownLeft", "browDownRight",
    "browInnerUp",
    "browOuterUpLeft", "browOuterUpRight",
    "cheekSquintLeft", "cheekSquintRight",
    "eyeSquintLeft", "eyeSquintRight",
    "eyeWideLeft", "eyeWideRight",
    "jawOpen",
    "mouthSmileLeft", "mouthSmileRight",
    "mouthFrownLeft", "mouthFrownRight",
    "mouthPressLeft", "mouthPressRight",
    "mouthShrugLower",
    "mouthUpperUpLeft", "mouthUpperUpRight",
    "noseSneerLeft", "noseSneerRight"
    };

    //Finished Flags
    public bool finishedNeutral = false;
    public bool finishedSmile = false;
    public bool finishedAngry = false;
    public bool finishedSad = false;
    public bool finishedSurprised = false;
    public bool finishedCalibration = false;

    public int collectedFrames = 0;
    public readonly int maxFrames = 180;

    void Start()
    {
        foreach (string relBs in relevantBlendshapes)
        {
            globalMax[relBs] = 0f;
        }

    }
    private void Update()
    {
        if (!provider.PythonReady || !isCalibrating)
        {
            return;
        }

        var blendshapes = provider.Blendshapes;
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
        SetCalibrationFlag(currentPhase);
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
        if (finishedNeutral &&
              finishedSmile &&
              finishedAngry &&
              finishedSad &&
              finishedSurprised)
        {
            finishedCalibration = true;

            gameStateManager.SetState(GameState.Gameplay);
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
    private void SetCalibrationFlag(EmotionCalibrationPhase calibrationPhase)
    {
        if (calibrationPhase == EmotionCalibrationPhase.Neutral)
        {
            finishedNeutral = true;
        }
        else if (calibrationPhase == EmotionCalibrationPhase.SmileMax)
        {
            finishedSmile = true;
        }
        else if (calibrationPhase == EmotionCalibrationPhase.AngryMax)
        {
            finishedAngry = true;
        }
        else if (calibrationPhase == EmotionCalibrationPhase.SadMax)
        {
            finishedSad = true;
        }
        else if (calibrationPhase == EmotionCalibrationPhase.SurprisedMax)
        {
            finishedSurprised = true;
        }
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
    public IReadOnlyDictionary<string, float> GetGobalMax() //Damit Werte nicht verfälscht werden können
    {
        return globalMax;
    }
    public IReadOnlyDictionary<string, float> GetNeutralBase()
    {
        return neutralBase;
    }


}
public enum EmotionCalibrationPhase
{
    None,
    Neutral,
    SmileMax,
    AngryMax,
    SadMax,
    SurprisedMax
}

