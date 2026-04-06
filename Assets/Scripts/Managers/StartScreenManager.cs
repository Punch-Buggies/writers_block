using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCam;
    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject bookshelfCam;

    bool firstTime;

    void Awake()
    {
        startScreenCam.SetActive(true);
        mainCam.SetActive(false);
        bookshelfCam.SetActive(false);

        firstTime = true;
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

        // if its your first time going to the bookshelf
        if (firstTime)
        {
            // display bookshelf info
            PurchaseManager.Instance.DisplayNoBooksPublished("first bookshelf"); // bad function name, this just calls tutorial display
            firstTime = false;
        }
    }
}
