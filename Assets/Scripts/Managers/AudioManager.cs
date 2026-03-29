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
    [Range(0f, 1f)] public float randomizerVolume = 1f;
    [Range(0f, 1f)] public float staggeredPagesVolume = 1f;

    [Header("Music")]
    [SerializeField] public AudioClip mainMusic; 
    [SerializeField] private List<SFXData> sfxList;
    // variables
    private Dictionary<string, SFXData> sfxDict;
    private Coroutine duckRoutine;
    private Coroutine staggeredPagesRoutine;
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
        // each sound will return its length
        float l1 = PlaySound("Audio/Randomizer/GENRE/GENRE_" + genre);
        float l2 = PlaySound("Audio/Randomizer/CHARACTER/CHARACTER_" + character);
        float l3 = PlaySound("Audio/Randomizer/SETTING/SETTING_" + setting);
        // Debug.Log("played all the sounds");

        // find the longest clip length and duck bgm for that amount of time
        float longest = Mathf.Max(l1, l2, l3);
        
        // if it already ducking, cancel and start this one
        if (duckRoutine != null)
        {
            StopCoroutine(duckRoutine);
        }
        duckRoutine = StartCoroutine(DuckBGM(longest));
    }

    private float PlaySound(string path)
    {
        // plays the clip found in the path
        // returns the length of the clip to be calcualted for ducking bgm
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
                return 0f;
            }
        }

        if (clip == null)
        {
            Debug.LogWarning("clip is null at: " + path);
        }

        Debug.Log($"Playing clip: {path} at volume {randomizerVolume}");
        sfxSource.PlayOneShot(clip, randomizerVolume);
        return clip.length;
    }

    public void PlaySFX(string sfxName)
    {
        if (!sfxDict.TryGetValue(sfxName, out var sfx)) return;
        if (sfx.clip == null) return;

        // Debug.Log($"playing a {sfx} clip");
        sfxSource.PlayOneShot(sfx.clip, sfx.volume);
    }

    public void StopSFXSounds()
    {
        if (staggeredPagesRoutine != null)
        {
            // stop playing page flipping
            StopCoroutine(staggeredPagesRoutine);
            staggeredPagesRoutine = null;
        }
        // stops unique book sounds
        sfxSource.Stop();
    }
     public void PlayStaggeredPages(int add)
    {
        staggeredPagesRoutine = StartCoroutine(PlayStaggeredCoroutine(add));
    }

    private IEnumerator PlayStaggeredCoroutine(int add)
    {
        string[] pageNames = {"page1", "page2", "page turn", "page3", "page4"};
        List<AudioClip> pageClips = new List<AudioClip>();
        foreach (string name in pageNames)
        {
            AudioClip clip = sfxDict[name].clip;
            for (int i = 0; i < add; i++)
            {
                pageClips.Add(clip);
            }
        }

        var shuffledClips = pageClips.OrderBy(x => Random.value);

        foreach (AudioClip clip in shuffledClips)
        {
            sfxSource.PlayOneShot(clip, staggeredPagesVolume);
            // stagger them by waiting a few seconds
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator DuckBGM(float duration)
    {
        float ogVolume = musicSource.volume;
        float duckedVolume = ogVolume * 0.3f;
        float fadeTime = 0.2f;

        Debug.Log("DUCKING DOWN");
        // fade down the bgm
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(ogVolume, duckedVolume, t / fadeTime);
            yield return null;
        }

        // stay ducked
        yield return new WaitForSeconds(duration);

        // fade up bgm
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(duckedVolume, ogVolume, t / fadeTime);
            yield return null;
        }
        // make sure to fully resetore volume
        musicSource.volume = ogVolume;
        Debug.Log("VOLUME RESTORED");
    }
    
    public void PlayBookCloseSound()
    {
        StopSFXSounds();
        PlayStaggeredPages(1);
        PlaySFX("book close");
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