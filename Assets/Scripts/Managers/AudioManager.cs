using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    [Header("Audio Sources")] // for inspector
    [SerializeField] private AudioSource musicSource; 
    [SerializeField] private AudioSource sfxSource; 

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // cache for audio clips
    private Dictionary<string, AudioCLip> clipCache = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // if it exists but its not this, destroy
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
           Debug.Log("ahhh the audio manager is being destroyed");
           Destroy(gameObject);
           return; 
        }
    }

    private void Start()
    {
        UpdateVolume();
    }

    // RANDOMIZER
    public void PlayUniqueBookSound(string genre, string character, string setting)
    {
        PlaySound("Audio/Randomizer/GENRE/GENRE_" + genre);
        PlaySound("Audio/Randomizer/CHARACTER/CHARACTER_" + genre);
        PlaySound("Audio/Randomizer/SETTING/SETTING_" + genre);
    }

    private void PlaySound(string path)
    {
        if (!clipCache.TryGetValue(path, out AudioClip clip))
        {
            // load clip
            clip = Resources.Load<AudioClip>(path);

            if (clip != null)
            {
                clipCache[path] = clip;
            }
            else
            {
                Debug.LogWarning"Audio not found at: " + path);
                return;
            }
        }

        sfxSource.PlayOneShot(clip);
    }

    // VOLUME

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateVolumes();
    }

    private void UpdateVolume()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

}