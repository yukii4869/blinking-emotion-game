using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkCondition : ICondition
{
    public bool IsMet => !isExtinguished;
    public string Description => "Nicht blinzeln!";
    private bool isBlinking = false;
    private bool isExtinguished = false;


    public BlinkCondition()
    {
        GameplayFaceInput.OnBlink += HandleBlink;
    }

    private void HandleBlink()
    {
        isBlinking = true;
        isExtinguished = true;
    }


}