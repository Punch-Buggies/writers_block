using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [SerializeField] GameObject pauseCam;
    [SerializeField] GameObject mainCam;
    [SerializeField] GameObject startCam;
    [SerializeField] GameObject bookshelfCam;
    [Header("Audio")]
    [SerializeField] Slider audioBGSlider;
    [SerializeField] Slider audioSFXSlider;
    [SerializeField] AudioClip adjustedAudioClip;
    [SerializeField] AudioSource adjustedBGAudioSource;
    [SerializeField] AudioSource adjustedSFXAudioSource;
    [SerializeField] AudioSource BGAudioSource;
    [SerializeField] AudioSource SFXAudioSource;
    [Header("Toggle Info ON/OFF")]
    [SerializeField] TextMeshProUGUI purchaseYesNo;
    [SerializeField] TextMeshProUGUI infoYesNo;


    [Header("GameObjects")]
    [SerializeField] GameObject quillTrail;
    [SerializeField] GameObject bookshelfUI;

    string currentScene;


    void Start()
    {
        audioBGSlider.value = 10f;
        currentScene = "Start";
    }

    public void TogglePause()
    {
        AudioManager.Instance.PlaySFX("click");
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        pauseCam.SetActive(true);
        mainCam.SetActive(false);
        startCam.SetActive(false);
        bookshelfCam.SetActive(false);

        BGAudioSource.Pause();

        Time.timeScale = 0f;
        IsPaused = true;

        quillTrail.SetActive(false);
        bookshelfUI.SetActive(false);
    }

    public void Resume()
    {
        AudioManager.Instance.PlaySFX("click");
        pauseCam.SetActive(false);
        switch (currentScene)
        {
            case "Start":
                startCam.SetActive(true);
                break;

            case "Main":
                mainCam.SetActive(true);
                break;

            case "Bookshelf":
                bookshelfCam.SetActive(true);
                bookshelfUI.SetActive(true);
                break;

            default:
                mainCam.SetActive(true);
                break;
        }
        BGAudioSource.Play();

        Time.timeScale = 1f;
        IsPaused = false;

        quillTrail.SetActive(true);
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlaySFX("click");

        PurchaseManager.Instance.ConfirmRestartGame((confirmed) =>
        {
            if (confirmed)
            {
                AudioManager.Instance.PlaySFX("quill");
                Time.timeScale = 1f;
                IsPaused = false;

                // Just Reload the scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        });
    }

    public void IncreaseBGVolume()
    {
        audioBGSlider.value = Mathf.Clamp(audioBGSlider.value + 1f, 0f, 10f);

        float normalizedVolume = audioBGSlider.value / 10f;

        adjustedBGAudioSource.volume = normalizedVolume;
        BGAudioSource.volume = normalizedVolume;

        adjustedBGAudioSource.PlayOneShot(adjustedAudioClip);
    }

    public void DecreaseBGVolume()
    {
        audioBGSlider.value = Mathf.Clamp(audioBGSlider.value - 1f, 0f, 10f);

        float normalizedVolume = audioBGSlider.value / 10f;

        adjustedBGAudioSource.volume = normalizedVolume;
        BGAudioSource.volume = normalizedVolume;

        adjustedBGAudioSource.PlayOneShot(adjustedAudioClip);
    }


    public void IncreaseSFXVolume()
    {
        audioSFXSlider.value = Mathf.Clamp(audioSFXSlider.value + 1f, 0f, 10f);

        float normalizedVolume = audioSFXSlider.value / 10f;

        adjustedSFXAudioSource.volume = normalizedVolume;
        SFXAudioSource.volume = normalizedVolume;

        adjustedSFXAudioSource.PlayOneShot(adjustedAudioClip);
    }

    public void DecreaseSFXVolume()
    {
        audioSFXSlider.value = Mathf.Clamp(audioSFXSlider.value - 1f, 0f, 10f);

        float normalizedVolume = audioSFXSlider.value / 10f;

        adjustedSFXAudioSource.volume = normalizedVolume;
        SFXAudioSource.volume = normalizedVolume;

        adjustedSFXAudioSource.PlayOneShot(adjustedAudioClip);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsPaused) Resume();
            else
                TogglePause();
        }
        TrackScene();
    }

    public bool ChangePurchaseText()
    {   
        // changes to either yes or no
        if (purchaseYesNo.text == "YES")
        {
            // return false if the text is NO, they don't want to toggle
            purchaseYesNo.text = "NO";   
            return false;
        }
        else
        {
            // return true if the text is YES, they do want to see info
            purchaseYesNo.text = "YES";
            return true;
        }
    }
    public bool ChangeInfoText()
    {
       // changes to either yes or no
        if (infoYesNo.text == "YES")
        {
            // return false if the text is NO, they don't want to toggle
            infoYesNo.text = "NO";   
            return false;
        }
        else
        {
            // return true if the text is YES, they do want to see info
            infoYesNo.text = "YES";
            return true;
        }
    }


    public void TrackScene()
    {
        if(startCam.activeSelf)
        {
            currentScene = "Start";
        }
        else if(mainCam.activeSelf)
        {
            currentScene = "Main";
        }
        else if(bookshelfCam.activeSelf)
        {
            currentScene = "Bookshelf";
        }
    }

}
