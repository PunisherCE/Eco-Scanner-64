using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip missileClip;
    [SerializeField] private AudioClip backgroundMusic;

    private void Awake()
    {
        // Singleton Pattern Implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    // --- Music Logic ---
    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // --- Public SFX Methods ---
    public void PlayShoot()
    {
        sfxSource.PlayOneShot(shootClip);
    }

    public void PlayExplosion()
    {
        sfxSource.PlayOneShot(explosionClip);
    }

    public void PlayMissile()
    {
        sfxSource.PlayOneShot(missileClip);
    }
}
