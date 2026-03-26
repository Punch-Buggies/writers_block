using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [SerializeField] GameObject pauseCam;
    [SerializeField] GameObject mainCam;
    [SerializeField] Slider audioSlider;
    [SerializeField] AudioClip adjustedAudioClip;
    [SerializeField] AudioSource adjustedAudioSource;
    [SerializeField] AudioSource musicAudioSource;

    void Start()
    {
        audioSlider.value = 10f;
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
        musicAudioSource.Pause();

        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        AudioManager.Instance.PlaySFX("click");
        pauseCam.SetActive(false);
        mainCam.SetActive(true);
        musicAudioSource.Play();

        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlaySFX("click");
        Time.timeScale = 1f;
        IsPaused = false;

        // Just Reload the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

public void IncreaseVolume()
{
    audioSlider.value = Mathf.Clamp(audioSlider.value + 1f, 0f, 10f);

    float normalizedVolume = audioSlider.value / 10f;

    adjustedAudioSource.volume = normalizedVolume;
    musicAudioSource.volume = normalizedVolume;

    adjustedAudioSource.PlayOneShot(adjustedAudioClip);
}

public void DecreaseVolume()
{
    audioSlider.value = Mathf.Clamp(audioSlider.value - 1f, 0f, 10f);

    float normalizedVolume = audioSlider.value / 10f;

    adjustedAudioSource.volume = normalizedVolume;
    musicAudioSource.volume = normalizedVolume;

    adjustedAudioSource.PlayOneShot(adjustedAudioClip);
}

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (IsPaused) Resume();
            else
                TogglePause();
        }
    }
}
