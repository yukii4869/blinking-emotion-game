using UnityEngine;
using System.Collections.Generic;
using System;

public class EARCalibrator : MonoBehaviour
{
    private readonly EARCalculator earCalculator = new();
    private bool isCalibrating;
    public bool finishedCalibration = false;
    private float calibrationTimer;
    private const float calibrationDuration = 2f;
    private readonly float lowerCutPercentage = 0.15f;
    private readonly float blinkFactor = 0.75f;
    private List<float> earSamples = new();
    public float neutralEAR;
    private bool startedCalibration;
    public float blinkThreshold;
    public bool StartedCalibration => startedCalibration;
    public event Action OnEARCalibrationFinished;

    private void Update()
    {
        if (!MediaPipeProvider.Instance.PythonReady || !isCalibrating)
        {
            return;
        }
        float currentEAR = earCalculator.ComputeBothEyes(MediaPipeProvider.Instance.Landmarks);

        earSamples.Add(currentEAR);

        calibrationTimer += Time.deltaTime;

        if (calibrationTimer >= calibrationDuration)
        {
            FinishCalibration();
        }
    }
    public void StartEARCalibration()
    {
        startedCalibration = true;
        earSamples.Clear();
        calibrationTimer = 0f;
        isCalibrating = true;
        finishedCalibration = false;
    }
    public void WriteEARToProfile(PlayerProfile profile)
    {
        profile.neutralEAR = neutralEAR;
        profile.blinkThreshold = blinkThreshold;
    }

    private void FinishCalibration()
    {
        isCalibrating = false;
        var sorted = SortList(earSamples);
        var cleaned = RemoveLowerPercentage(sorted);
        neutralEAR = ComputeNeutralEAR(cleaned);
        blinkThreshold = ComputeBlinkThreshold(neutralEAR);
        finishedCalibration = true;
        OnEARCalibrationFinished?.Invoke();
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
        foreach (float ear in removedSamples)
        {
            accumulatedEAR += ear;
        }
        float meanEAR = accumulatedEAR / lengthcleanedList;
        return meanEAR;
    }
    private float ComputeBlinkThreshold(float meanEAR)
    {
        return meanEAR * blinkFactor;
    }
    public void LoadFromProfile(PlayerProfile profile)
    {
        neutralEAR = profile.neutralEAR; // oder dein Dictionary
        blinkThreshold = profile.blinkThreshold;

        finishedCalibration = true;
    }

}