using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] private AudioClip musicClip;
    [Range(0f, 1f)] [SerializeField] private float volume = 0.25f;

    private AudioSource audioSource;
    private bool wasPausedByGame = false;

    private void Awake()
    {
        // Singleton pattern: persist this object across all scene changes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.playOnAwake = true;

        if (musicClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("[BackgroundMusicManager] No AudioClip assigned. Please drag an audio file (.mp3, .wav) into the Music Clip slot in the Inspector.");
        }
    }

    private void Update()
    {
        if (audioSource == null || musicClip == null) return;

        // Dynamically pause/unpause background music based on Time.timeScale (pauses during ECS menu)
        if (Time.timeScale == 0f)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                wasPausedByGame = true;
                Debug.Log("[BackgroundMusicManager] Music paused due to game suspension.");
            }
        }
        else
        {
            if (wasPausedByGame && !audioSource.isPlaying)
            {
                audioSource.UnPause();
                wasPausedByGame = false;
                Debug.Log("[BackgroundMusicManager] Music resumed.");
            }
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        musicClip = newClip;
        if (audioSource != null)
        {
            audioSource.clip = musicClip;
            if (musicClip != null)
            {
                audioSource.Play();
                wasPausedByGame = false;
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
