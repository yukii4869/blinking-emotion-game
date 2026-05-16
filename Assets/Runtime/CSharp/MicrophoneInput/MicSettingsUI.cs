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
        float loudness = MicInputManager.Instance.GetLoudness();
        smoothedLoudness = Mathf.Lerp(smoothedLoudness, loudness, Time.deltaTime * smoothSpeed);
        // Live-Lautstärke anzeigen

        // Pegel-Balken füllen (verstärkt, weil Werte klein sind)
        volumeBar.fillAmount = Mathf.Clamp01(smoothedLoudness * 10f);


        // Farbe interpolieren
        volumeBar.color = Color.Lerp(Color.green, Color.red, smoothedLoudness * 10f);
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
