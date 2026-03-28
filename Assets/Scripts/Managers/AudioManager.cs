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
    [SerializeField] public AudioClip quillSFX; 
    [SerializeField] public AudioClip eraseSFX; 
    [SerializeField] public AudioClip clickSFX;
    [SerializeField] public AudioClip purchaseSFX;
    [SerializeField] public AudioClip growFinishSFX;
    [SerializeField] public AudioClip dropSFX;
    [SerializeField] public AudioClip pageSFX;
    [SerializeField] public AudioClip page1;
    [SerializeField] public AudioClip page2;
    [SerializeField] public AudioClip page3;
    [SerializeField] public AudioClip page4;
    [SerializeField] public AudioClip shortPage;

    private Dictionary<string, AudioClip> sfxDict;
    

    // cache for audio clips
    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

    private void MakeSFXDict()
    {
        sfxDict = new Dictionary<string, AudioClip>
        {
            {"quill", quillSFX},
            {"eraser", eraseSFX},
            {"click", clickSFX},
            {"purchase", purchaseSFX},
            {"grow finish", growFinishSFX},
            {"drop", dropSFX},
            {"page turn",pageSFX},
            {"increment", page3},
            {"decrement", page4},
            {"shortPage", shortPage}
        };

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
    public void PlaySFX(string sfx)
    {
        AudioClip sfxClip = sfxDict[sfx];
        if (sfxClip == null) return;

        // Debug.Log($"playing a {sfx} clip");
        sfxSource.clip = sfxClip;
        sfxSource.Play();
    }

    public void PlayStaggeredPages()
    {
        StartCoroutine(PlayStaggeredCoroutine());
    }

    private IEnumerator PlayStaggeredCoroutine()
    {
        AudioClip[] pageClips = {page1, page2, pageSFX, page3, page4};

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