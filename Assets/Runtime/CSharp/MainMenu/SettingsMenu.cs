using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlsPanel;

    private void Start()
    {
        
    }

    public void ShowGameplay()
    {
        gameplayPanel.SetActive(true);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowGraphics()
    {
        gameplayPanel.SetActive(false);
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowAudio()
    {
        gameplayPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void ShowControls()
    {
        gameplayPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
}
