using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MicSettingsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image volumeBar;
    [SerializeField] private TMP_Dropdown micDropdown;
    private int chosenDeviceIndex = 0;
    public static UnityAction<int> OnMicrophoneChoiceChanged;
    private float smoothedLoudness;
    private float smoothSpeed = 10f;

    private void Start()
    {

        micDropdown.onValueChanged.AddListener(OnMicDeviceChanged);

        PopulateSourceDropDown();
        micDropdown.onValueChanged.AddListener(OnMicDeviceChanged);

    }

    private void Update()
    {
        if (MicInputManager.Instance == null)
            return;

        float loudness = MicInputManager.Instance.smoothedLoudness;

        smoothedLoudness = Mathf.Lerp(smoothedLoudness, loudness, Time.unscaledDeltaTime * smoothSpeed);


        volumeBar.fillAmount = Mathf.Clamp01(smoothedLoudness);
        volumeBar.color = Color.Lerp(Color.green, Color.red, smoothedLoudness);
    }
    private void PopulateSourceDropDown()
    {
        var options = new List<TMP_Dropdown.OptionData>();

        foreach (var microphone in Microphone.devices)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(microphone, null, Color.white);
            options.Add(optionData);
        }

        micDropdown.options = options;
    }
    private void OnMicDeviceChanged(int optionIndex)
    {
        chosenDeviceIndex = optionIndex;
        PlayerPrefs.SetInt("SelectedMicIndex", optionIndex);
        PlayerPrefs.Save();
        OnMicrophoneChoiceChanged?.Invoke(chosenDeviceIndex);
    }
}
