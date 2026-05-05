using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadProfileUI : MonoBehaviour
{
    [SerializeField] private List<ProfileSlot> slots;

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
        ActiveProfile.Instance.SetProfile(profile);
        MediaPipeProvider.Instance.StartMediaPipe();
        GameSceneManager.Instance.LoadGame();
    }

    private void DeleteProfile(string name)
    {
        ProfileManager.DeleteProfile(name);
        RefreshSlots();
    }

    private void StartNewCalibration()
    {
        MediaPipeProvider.Instance.StartMediaPipe();
        GameSceneManager.Instance.LoadCalibration();
        gameObject.SetActive(false);
    }
}
