using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // For background music (looping)
    [SerializeField] private AudioSource sfxSource; // For one-shot music (victory/defeat)

    [Range(0, 1)] public float musicVolume = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create and configure audio sources
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Background music settings
        bgmSource.loop = true;
        bgmSource.volume = musicVolume;

        // SFX music settings
        sfxSource.loop = false;
        sfxSource.volume = musicVolume;
    }

    public void PlayBackgroundMusic()
    {
        // Only restart if not already playing
        if (bgmSource.clip != backgroundMusic || !bgmSource.isPlaying)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.Play();
        }
    }

    public void PlayVictoryMusic()
    {
        bgmSource.Stop(); // Stop background music
        sfxSource.clip = victoryMusic;
        sfxSource.Play();
    }

    public void PlayDefeatMusic()
    {
        bgmSource.Stop(); // Stop background music
        sfxSource.clip = defeatMusic;
        sfxSource.Play();
    }
}