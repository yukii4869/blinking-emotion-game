using UnityEngine;

public class ActiveProfile : MonoBehaviour
{
    public static ActiveProfile Instance { get; private set; }

    public PlayerProfile CurrentProfile { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetProfile(PlayerProfile profile)
    {
        CurrentProfile = profile;
    }
}
