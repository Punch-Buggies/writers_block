using System.Diagnostics.Contracts;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// This script is attached to the MoneyUI gameObject btw
/// </summary>
public class PurchaseManager : MonoBehaviour
{
    // only moneymanager script can set moneymanager
    public static PurchaseManager Instance { get; private set;}

    [Header("Canvases")]
    [SerializeField] private CanvasGroup mainUIGroup;      // MainUI CanvasGroup
    [SerializeField] private CanvasGroup bookshelfGroup;      // bookshelf/bookview CanvasGroup
    [SerializeField] private GameObject bookshelfOverlay;
    [SerializeField] private CanvasGroup pauseGroup;      // pause menu CanvasGroup

    [Header("Purchase View")]
    [SerializeField] GameObject PurchaseUIView;
    [SerializeField] TextMeshProUGUI uiText;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button okButton;
    [SerializeField] GameObject dontShowAgain;

    [Header("Toggle On View (Purchase)")]
    [SerializeField] Image checkMarkPurchase;
    [SerializeField] Image boxClickPurchase;
    [Header("Toggle On View (Info)")]
    [SerializeField] Image checkMarkInfo;
    [SerializeField] Image boxClickInfo;

    // show/unshow checkmark 
    // if shown, don't siaplay anumore
    public bool toggleOnPurchase = true; // show the purchase ui or not
    public bool toggleOnInfo = true; // show the purchase ui or not

    private Action<bool> onConfirm; //stores which button the player clicked
    PauseManager pauseManager;

    void Awake()
    {
        // if it exists but its not this, destroy
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);

        }
        else if (Instance != this)
        {
           Debug.Log("ahhh the purhcasemanagher os being destroyed");
           Destroy(gameObject);
           return; 
        }
        // button listeners
        yesButton.onClick.AddListener( () => Respond(true));
        noButton.onClick.AddListener( () => Respond(false));
        pauseManager = FindAnyObjectByType<PauseManager>();

        // set the dont show again color to be dark

    }

    private void DimUI(CanvasGroup UIGroup)
    {
        if (UIGroup != null)
        {
            if (UIGroup != bookshelfGroup)
            {
                UIGroup.alpha = 0.5f;
            }
            else if (UIGroup == bookshelfGroup)
            {
                // turn on dark overlay on bookshelf
                bookshelfOverlay.SetActive(true);
            }
            // block interactions and raycasts
            UIGroup.interactable = false;
            UIGroup.blocksRaycasts = false;
        }   
        return;
    }

    public void UndimMainUI()
    {
        // this is attached to the yes/no buttons
        if (mainUIGroup != null)
        {
            mainUIGroup.alpha = 1f;
            mainUIGroup.interactable = true;
            mainUIGroup.blocksRaycasts = true;
        }   
        return;
    }
    public void UndimBookshelfUI()
    {
        if (bookshelfGroup != null)
        {
            // bookshelfGroup.alpha = 0f;
            bookshelfOverlay.SetActive(false);
            bookshelfGroup.interactable = true;
            bookshelfGroup.blocksRaycasts = true;
        }   
        return;
    }
    public void UndimPauseUI()
    {
        // this is attached to the yes/no buttons
        if (pauseGroup != null)
        {
            pauseGroup.alpha = 1f;
            pauseGroup.interactable = true;
            pauseGroup.blocksRaycasts = true;
        }   
        return;
    }

    public void ConfirmTileGrowthPayment(StoryElement storyElement, float time, double cost, Action<bool> response)
    {
        // only ask to confirm if toggle is on, otherwise assume true
        // if ()
        // 1. Populate the textbox with the necessary information
        uiText.text = $"Would you like to spend {time} seconds growing the {storyElement.GetElementType()} thought for ${cost}?";

        // make sure the checkmark is the purchase one not the info checkmark
        // also make sure the right clcikable box is there
        boxClickPurchase.gameObject.SetActive(true);
        checkMarkPurchase.gameObject.SetActive(!toggleOnPurchase);
      
        boxClickInfo.gameObject.SetActive(false);
        checkMarkInfo.gameObject.SetActive(false);

        // set the right do not show again text
        dontShowAgain.GetComponentInChildren<TMP_Text>().text = "Don't Show Purchase Info Again";

        // 2. Dim and turn off interacbles of main UI
        DimUI(mainUIGroup);

        // 3. Display the UI
        RectTransform rect = PurchaseUIView.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -3.65425f);
        PurchaseUIView.SetActive(true);

        // 4. Respond to their decision
        onConfirm = response;
    }

    public void ConfirmRestartGame(Action<bool> response)
    {
        uiText.text = "Are you sure you want to restart? You will lose all published books.";

        // turn off all checkmarks, boxes, and dont show again
        boxClickPurchase.gameObject.SetActive(false);
        checkMarkPurchase.gameObject.SetActive(false);
        boxClickInfo.gameObject.SetActive(false);
        checkMarkInfo.gameObject.SetActive(false);
        dontShowAgain.gameObject.SetActive(false);

        DimUI(pauseGroup);

        // make sure its in the pause menu
        RectTransform rect = PurchaseUIView.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -42.5f);

        PurchaseUIView.SetActive(true);

        onConfirm = response;
    }
    private void Respond(bool confirmed)
    {   // this is called when either yes or no button is clicked
        AudioManager.Instance.PlaySFX("click");

        // 1. turn off view
        PurchaseUIView.SetActive(false);

        // 2. telling confirmed if its true or not
        onConfirm?.Invoke(confirmed); // ?. means only do the thing if onConfirm is not null

        print("undimmed the ui");

        // 3. clear the action
        onConfirm = null;
    }

    public void DisplayInsufficientFunds()
    {
        // 1. Dim MainUI
        DimUI(mainUIGroup);

        // 2. Popoulate text with insufficient funds message
        uiText.text = $"Sorry, you have insufficient funds.";

        // 3.1 Remove yes and no buttons
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        dontShowAgain.SetActive(false);
        // 3.2 Display ok button
        okButton.gameObject.SetActive(true);

        // 4. turn on view
        RectTransform rect = PurchaseUIView.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -3.65425f);
        PurchaseUIView.SetActive(true);
    }

    public void DisplayNoBooksPublished(string item)
    {
        // THIS IS FOR THE TUTORIAL, YOU CANNOT TURN THIS OFF BC THE PLAYER NEEDS TO KNOW
        // this function/display is actually about the fact that you can't turn it off and it only gives you the okay option
        // 1. Dim MainUI
        if (item == "first bookshelf")
        {
            DimUI(bookshelfGroup);
            // DimUI(mainUIGroup);
        }
        else
        {
            DimUI(mainUIGroup);
        }

        // 2. Popoulate text with insufficient funds message
        if (item == "eraser")
        {
            uiText.text = "You cannot erase any thoughts until you have published 1 book.";
        }
        else if (item == "first book")
        {
            uiText.text = "Congratulations on publishing your first story! Click on the bookshelf to see the collection of stories you've written.";
        }
        else if (item == "first bookshelf")
        {
            if (BookshelfManager.Instance.getBookCount() == 0)
            {
                uiText.text = $"This bookshelf will hold the collection of stories you write. When you've published your first book, click them to read your story.";
            }
            else
            {
                uiText.text = $"This bookshelf holds your collection of published stories. Click them to read.";   
            }
        }
        else
        {
            uiText.text = $"You cannot purchase a {item} until you have published 1 book.";
        }
       

        // 3.1 Remove yes and no buttons
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        dontShowAgain.SetActive(false);
        // 3.2 Display ok button
        okButton.gameObject.SetActive(true);

        // 4.1 make purchase view higher if its bookshelf 27.5 else -3.65425
        RectTransform rect = PurchaseUIView.GetComponent<RectTransform>();
        if (item == "first bookshelf")
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 27.5f);
        }
        else
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -3.65425f);
        }
        // 4.2 turn on view
        PurchaseUIView.SetActive(true);
    }

    public void DisplayTileInfo(string text)
    {
        // THIS IS TO HELP THE PLAYER FIGURE SUM SHIT OUT
        // CAN TURN OFF BC MAYNE ANNOYING
        // 1. Dim MainUI
        DimUI(mainUIGroup);

        // 2. Popoulate text with insufficient funds message
        uiText.text = text;

        // 3.1 Remove yes and no buttons
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        dontShowAgain.SetActive(true);
        dontShowAgain.GetComponentInChildren<TMP_Text>().text = "Don't Show Tile Info Again";
        // make sure its the info checkmark and not the pruchase one
        // also make sure the right clcikable box is there
        checkMarkInfo.gameObject.SetActive(!toggleOnInfo);
        boxClickInfo.gameObject.SetActive(true);

        checkMarkPurchase.gameObject.SetActive(false);
        boxClickPurchase.gameObject.SetActive(false);

        // 3.2 Display ok button
        okButton.gameObject.SetActive(true);

        // 4. turn on view
        // make sure view y position is set in the mainui
        RectTransform rect = PurchaseUIView.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -3.65425f);
        PurchaseUIView.SetActive(true);
    }



    public void ResetPurchaseUI()
    {
        // this is called in the ok button click
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        dontShowAgain.SetActive(true);
        // 3.2 Display ok button
        okButton.gameObject.SetActive(false);
    }

    public void PauseMenuClicked(string type)
    {
        // this is for when you are trying to change the settings in the pause menu
        // first change the text, and grab the toggleOn boolean, then turn off according checkmark and set toggle value

        AudioManager.Instance.PlaySFX("click");
        // the thing was clicked, change text
        bool toggleOn;
        // this is the dont show again checkmark, need to figure out if its the purfhase or info
        Image checkMarkImage;
        // get the toggle value and the right checkmark
        switch(type)
        {
            // set the right toggleON
            case "Purchase":
                toggleOn = pauseManager.ChangePurchaseText();
                checkMarkImage = checkMarkPurchase;
                break;
            case "Info":
                toggleOn = pauseManager.ChangeInfoText();
                checkMarkImage = checkMarkInfo;
                break;
            default:
                Debug.LogWarning($"Not a valid type: {type}");
                return;
        }
        // remove checkmark regardless (bc even if its toggleOFF they wouldnt see it)
        bool checkmarkIsOn = checkMarkImage.gameObject.activeSelf;
        checkMarkImage.gameObject.SetActive(!checkmarkIsOn); // this should be set to the opposite of whatever toggleOn is, if toggleOn is true, then checkmark should be false

        // set the right purchase or info toggle settings according to if its yes or no
        switch(type)
        {
            // get the right toggle variable
            case "Purchase":
                toggleOnPurchase = toggleOn;
                break;
            case "Info":
                toggleOnInfo = toggleOn;
                break;
            default:
                Debug.LogWarning($"Not a valid type: {type}");
                return;
        }
    }
}