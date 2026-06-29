using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadProfileUI : MonoBehaviour
{
    [SerializeField] private List<ProfileSlot> slots;

    [Header("Pagination")]
    [SerializeField] private int slotsPerPage = 6;
    [SerializeField] private GameObject nextPageButton;
    [SerializeField] private GameObject previousPageButton;

    private int currentPage = 0;
    private int totalPages = 0;

    private void OnEnable()
    {
        currentPage = 0;
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        var profiles = ProfileManager.GetAllProfiles();

        // Anzahl Seiten berechnen
        totalPages = Mathf.CeilToInt((float)profiles.Length / slotsPerPage);

        // Page clamping
        currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, totalPages - 1));

        // Startindex der aktuellen Seite
        int startIndex = currentPage * slotsPerPage;

        // Slots füllen
        for (int i = 0; i < slots.Count; i++)
        {
            int profileIndex = startIndex + i;

            if (profileIndex < profiles.Length)
            {
                string name = profiles[profileIndex];
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

        UpdatePageButtons();
    }

    private void UpdatePageButtons()
    {
        // Buttons aktivieren/deaktivieren
        previousPageButton.SetActive(currentPage > 0);
        nextPageButton.SetActive(currentPage < totalPages - 1);
    }

    public void NextPage()
    {
        currentPage++;
        RefreshSlots();
    }

    public void PreviousPage()
    {
        currentPage--;
        RefreshSlots();
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
