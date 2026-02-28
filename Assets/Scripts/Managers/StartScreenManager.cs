using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCam;
    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject bookshelfCam;


    void Awake()
    {
        startScreenCam.SetActive(true);
        mainCam.SetActive(false);
        bookshelfCam.SetActive(false);
    }


    public void StartGame()
    {
        startScreenCam.SetActive(false);
        mainCam.SetActive(true);
        bookshelfCam.SetActive(false);
    }

    public void GoToBookshelf()
    {
        startScreenCam.SetActive(false);
        mainCam.SetActive(false);
        bookshelfCam.SetActive(true);
    }
}
