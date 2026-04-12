using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using JetBrains.Annotations;

public class EARCalibrator : MonoBehaviour
{
    [SerializeField] MediaPipeProvider provider;
    private readonly EARCalculator eARCalculator = new();
    private bool isCalibrating;
    public bool finishedCalibration = false;
    private int collectedFrames = 0;
    private readonly int maxFrames = 50;
    private readonly float lowerCutPercentage = 0.15f;
    private readonly float blinkFactor = 0.75f;
    private List<float> earSamples = new();
    public float neutralEAR;
    public float blinkThreshold;

    private void Update()
    {
        if (!provider.pythonReady || !isCalibrating)
        {
            return;
        }
        if (collectedFrames < maxFrames)
        {
            float currentEAR = eARCalculator.ComputeBothEyes(provider.Landmarks);
            earSamples.Add(currentEAR);
            collectedFrames++;
        }
        else
        {
            FinishCalibration();
        }
    }
    public void StartEARCalibration()
    {
        earSamples.Clear();
        collectedFrames = 0;
        isCalibrating = true;
        finishedCalibration = false;
    }

    private void FinishCalibration()
    {
        isCalibrating = false;
        var sorted = SortList(earSamples);
        var cleaned = RemoveLowerPercentage(sorted);
        neutralEAR = ComputeNeutralEAR(cleaned);
        blinkThreshold = ComputeBlinkThreshold(neutralEAR);
        finishedCalibration = true;
    }
    private List<float> SortList(List<float> earValues)
    {
        List<float> sortedList = new(earValues);
        sortedList.Sort(); // [0.10, 0.12, 0.27, 0.29, 0.30]
        return sortedList;
    }
    private List<float> RemoveLowerPercentage(List<float> sortedValues)
    {
        int lengthList = sortedValues.Count;
        int cutCount = Mathf.FloorToInt(lengthList * lowerCutPercentage);
        return sortedValues.GetRange(cutCount, sortedValues.Count - cutCount);
    }
    private float ComputeNeutralEAR(List<float> removedSamples)
    {
        float accumulatedEAR = 0;
        int lengthcleanedList = removedSamples.Count;
        foreach(float ear in removedSamples)
        {
            accumulatedEAR += ear;
        }
         float meanEAR = accumulatedEAR/lengthcleanedList;
         return meanEAR;
    }
    private float ComputeBlinkThreshold(float meanEAR)
    {
        return meanEAR * blinkFactor;
    }
}