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

    [SerializeField] private CanvasGroup mainUIGroup;      // MainUI CanvasGroup

    [SerializeField] GameObject PurchaseUIView;
    [SerializeField] TextMeshProUGUI uiText;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button okButton;
    [SerializeField] Image checkMarkImage;
    [SerializeField] GameObject dontShowAgain;
    // show/unshow checkmark 
    // if shown, don't siaplay anumore
    public bool toggleOn = true; // show the purchase ui or not

    private Action<bool> onConfirm; //stores which button the player clicked

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

        // set the dont show again color to be dark

    }

    private void DimMainUI()
    {
        if (mainUIGroup != null)
        {
            mainUIGroup.alpha = 0.5f;
            mainUIGroup.interactable = false;
            mainUIGroup.blocksRaycasts = false;
        }   
        return;
    }

    public void UndimMainUI()
    {
        if (mainUIGroup != null)
        {
            mainUIGroup.alpha = 1f;
            mainUIGroup.interactable = true;
            mainUIGroup.blocksRaycasts = true;
        }   
        return;
    }
    public void ConfirmTileGrowthPayment(StoryElement storyElement, float time, double cost, Action<bool> response)
    {
        // only ask to confirm if toggle is on, otherwise assume true
        // if ()
        // 1. Populate the textbox with the necessary information
        uiText.text = $"Would you like to spend {time} seconds growing the {storyElement.GetElementType()} thought for ${cost}?";

        // 2. Dim and turn off interacbles of main UI
        DimMainUI();

        // 3. Display the UI
        PurchaseUIView.SetActive(true);

        // 4. Respond to their decision
        onConfirm = response;
    }

    private void Respond(bool confirmed)
    {   // this is called when either button is clicked
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
        DimMainUI();

        // 2. Popoulate text with insufficient funds message
        uiText.text = $"Sorry, you have insufficient funds.";

        // 3.1 Remove yes and no buttons
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        dontShowAgain.SetActive(false);
        // 3.2 Display ok button
        okButton.gameObject.SetActive(true);

        // 4. turn on view
        PurchaseUIView.SetActive(true);
    }

    public void DisplayNoBooksPublished(string item)
    {
        // 1. Dim MainUI
        DimMainUI();

        // 2. Popoulate text with insufficient funds message
        uiText.text = $"You cannot purchase a {item} until you have published 1 book.";

        // 3.1 Remove yes and no buttons
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        dontShowAgain.SetActive(false);
        // 3.2 Display ok button
        okButton.gameObject.SetActive(true);

        // 4. turn on view
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


    public void BoxClicked()
    {
        // the box was clicked, show checkmark depending
        // the checkmark is NOT there
        AudioManager.Instance.PlaySFX("click");
        if (checkMarkImage.gameObject.activeSelf == false)
        {
            checkMarkImage.gameObject.SetActive(true);
            // we do not want to show the purchase view
            toggleOn = false;
        }
        else //checkmark IS there
        {
            checkMarkImage.gameObject.SetActive(false);
            // we do want to show the purchase view
            toggleOn = true;
        }
    }


}