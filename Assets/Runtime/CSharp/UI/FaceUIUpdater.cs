using System;
using UnityEngine;
public class FaceUIManager : MonoBehaviour
{
    [SerializeField] private LiveFaceUI liveUI;

    private void Update()
    {
        UpdateRuntimeUI();
    }

    private void UpdateRuntimeUI()
    {
        if (!liveUI.gameObject.activeSelf)
        {
            return;
        }
        liveUI.UpdateBlinkCount();
        liveUI.UpdateEmotion();
    }
}