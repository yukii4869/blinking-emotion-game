using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadProfileUI : MonoBehaviour
{
    [SerializeField] private List<ProfileSlot> slots;
    [SerializeField] private ProfileLoader loader;

    private void OnEnable()
    {
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        var profiles = ProfileManager.GetAllProfiles();

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < profiles.Length)
            {
                string name = profiles[i];
                slots[i].SetupFilled(
                    name,
                    onLoad: () => LoadProfile(name),
                    onDelete: () => DeleteProfile(name)
                );
            }
            else
            {
                slots[i].SetupEmpty(
                    onNew: StartNewCalibration
                );
            }
        }
    }

    private void LoadProfile(string name)
    {
        var profile = ProfileManager.LoadProfile(name);
        loader.ApplyProfile(profile);
        ActiveProfile.Instance.SetProfile(profile);
        SceneManager.LoadScene("GameScene");
        gameObject.SetActive(false);
    }

    private void DeleteProfile(string name)
    {
        ProfileManager.DeleteProfile(name);
        RefreshSlots();
    }

    private void StartNewCalibration()
    {
        Debug.Log("New Calibration");
        CalibrationStateManager.Instance.SetState(CalibrationState.EARCalibration);
        gameObject.SetActive(false);
    }
}
