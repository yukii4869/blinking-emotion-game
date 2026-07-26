using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;      // 🎵 Musik
    [SerializeField] private AudioSource sfxSource;        // 🔊 One-Shot SFX
    [SerializeField] private AudioSource ambientSource;    // 🌫️ Ambient / Loop-SFX

    [Header("Sound Library")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    [Header("Volume")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 1f;

    private Dictionary<string, Sound> soundLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLookup();
    }

    private void BuildLookup()
    {
        soundLookup = new Dictionary<string, Sound>();

        foreach (var s in sounds)
        {
            if (string.IsNullOrEmpty(s.name) || s.clip == null)
            {
                Debug.LogWarning("AudioManager: Sound ohne Name oder Clip übersprungen.");
                continue;
            }

            if (!soundLookup.TryAdd(s.name, s))
            {
                Debug.LogWarning($"AudioManager: Doppelter Sound-Name '{s.name}'");
            }
        }
    }

    // ---------------------------------------------------------
    // ONE-SHOT SFX
    // ---------------------------------------------------------
    public void PlaySFX(string soundName)
    {
        if (!soundLookup.TryGetValue(soundName, out var sound))
        {
            Debug.LogWarning($"AudioManager: SFX '{soundName}' nicht gefunden.");
            return;
        }

        sfxSource.pitch = sound.pitch;
        sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume);
    }

    // ---------------------------------------------------------
    // MUSIC
    // ---------------------------------------------------------
    public void PlayMusic(string soundName, bool loop = true)
    {
        if (!soundLookup.TryGetValue(soundName, out var sound))
        {
            Debug.LogWarning($"AudioManager: Musik '{soundName}' nicht gefunden.");
            return;
        }

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume * musicVolume;
        musicSource.pitch = sound.pitch;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeOutRoutine(musicSource, duration));
    }

    // ---------------------------------------------------------
    // AMBIENT / LOOP-SFX
    // ---------------------------------------------------------
    public void PlayAmbient(string soundName)
    {
        if (!soundLookup.TryGetValue(soundName, out var sound))
        {
            Debug.LogWarning($"AudioManager: Ambient '{soundName}' nicht gefunden.");
            return;
        }

        ambientSource.clip = sound.clip;
        ambientSource.volume = sound.volume * ambientVolume;
        ambientSource.pitch = sound.pitch;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void FadeOutAmbient(float duration = 1f)
    {
        StartCoroutine(FadeOutRoutine(ambientSource, duration));
    }

    // ---------------------------------------------------------
    // Fade-Out Routine (für Music & Ambient)
    // ---------------------------------------------------------
    private IEnumerator FadeOutRoutine(AudioSource src, float duration)
    {
        float startVolume = src.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            src.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        src.Stop();
        src.volume = startVolume;
    }
}
