using System;
using UnityEngine;

public class ModeSelectionMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadProfileUI;
    [SerializeField] private GameObject modeSelectionMenuUI;
    public void SelectFaceNormal()
    {
        GlobalModeStorage.Instance.SelectedMode = GameMode.FaceNormal;
        modeSelectionMenuUI.SetActive(false);
        loadProfileUI.SetActive(true);
    }

    public void SelectKeyboard()
    {
        GlobalModeStorage.Instance.SelectedMode = GameMode.Keyboard;
        modeSelectionMenuUI.SetActive(false);
        loadProfileUI.SetActive(true);
    }

    public void SelectFaceNoFeedback()
    {
        GlobalModeStorage.Instance.SelectedMode = GameMode.FaceNoFeedback;
        modeSelectionMenuUI.SetActive(false);
        loadProfileUI.SetActive(true);
    }

    public void SelectFaceNoCalibration()
    {
        GlobalModeStorage.Instance.SelectedMode = GameMode.FaceNoCalibration;
        modeSelectionMenuUI.SetActive(false);
        loadProfileUI.SetActive(true);
    }
}
