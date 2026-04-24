using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LoadProfileUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown profileDropdown;
    [SerializeField] private GameStateManager gsm;


    private List<string> profiles;

    [SerializeField] private ProfileLoader loader;

    public void OnLoadPressed()
    {
        string selected = profiles[profileDropdown.value];
        PlayerProfile profile = ProfileManager.LoadProfile(selected);

        loader.ApplyProfile(profile);

        gameObject.SetActive(false);
        gsm.SetState(GameState.Gameplay);
    }
        public void OnNewCalibrationPressed()
    {
        gameObject.SetActive(false);
        gsm.SetState(GameState.EARCalibration);
    }

}
