using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCam;
    [SerializeField] private GameObject mainCam;

    void Start()
    {
        startScreenCam.SetActive(true);
        mainCam.SetActive(false);
    }


    public void StartGame()
    {
        startScreenCam.SetActive(false);
        mainCam.SetActive(true);
    }
}
