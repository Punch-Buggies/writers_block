using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    [Header("Audio Sources")] // for inspector
    [SerializeField] private AudioSource musicSource; 
    [SerializeField] private AudioSource sfxSource; 

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Music")]
    [SerializeField] public AudioClip mainMusic; 

    [SerializeField] private List<SFXData> sfxList;

    private Dictionary<string, SFXData> sfxDict;
    

    // cache for audio clips
    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();
    // DATA TYPE FOR AUDIO CLIPS
    [System.Serializable]
    public class SFXData
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
    }

    private void MakeSFXDict()
    {
        // put at all the sfx in the list into the dictionary
        sfxDict = new Dictionary<string, SFXData>();
        foreach (var sfx in sfxList)
        {
            if (!sfxDict.ContainsKey(sfx.name))
            {
                sfxDict.Add(sfx.name, sfx);
            }
            else
            {
                Debug.LogWarning($"Duplicate SFX name: {sfx.name}");
            }
        }
    }
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
        // Debug.Log("audio manager starting up");
        UpdateVolume();
        PlayMusic(mainMusic);
        MakeSFXDict();
        // Debug.Log("main music is playing now");
    }


    // BG MUSIC
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return; // already playing

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = 1f;
        musicSource.Play();
    }


    // RANDOMIZER
    public void PlayUniqueBookSound(string genre, string character, string setting)
    {
        PlaySound("Audio/Randomizer/GENRE/GENRE_" + genre);
        PlaySound("Audio/Randomizer/CHARACTER/CHARACTER_" + character);
        PlaySound("Audio/Randomizer/SETTING/SETTING_" + setting);
        // Debug.Log("played all the sounds");
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
                Debug.LogWarning("Audio not found at: " + path);
                return;
            }
        }

        if (clip == null)
        {
            Debug.LogWarning("clip is null at: " + path);
        }

        // Debug.Log("Playing clip: " + path);
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(string sfxName)
    {
        if (!sfxDict.TryGetValue(sfxName, out var sfx)) return;
        if (sfx.clip == null) return;

        // Debug.Log($"playing a {sfx} clip");
        sfxSource.PlayOneShot(sfx.clip, sfx.volume);
    }

    public void PlayStaggeredPages()
    {
        StartCoroutine(PlayStaggeredCoroutine());
    }

    private IEnumerator PlayStaggeredCoroutine()
    {
        string[] pageNames = {"page1", "page2", "page turn", "page3", "page4"};
        List<AudioClip> pageClips = new List<AudioClip>();
        foreach (string name in pageNames)
        {
            AudioClip clip = sfxDict[name].clip;
            pageClips.Add(clip);
        }

        var shuffledClips = pageClips.OrderBy(x => Random.value);

        foreach (AudioClip clip in shuffledClips)
        {
            sfxSource.PlayOneShot(clip);
            yield return new WaitForSeconds(0.2f);
        }
    }


    public void playTest()
    {
        // PlayUniqueBookSound("Romance","Antihero","Mountains");
        PlaySound("Audio/SFX/Farming/Farming_Eraser");
        Debug.Log("test over");
    }

    // VOLUME

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

}