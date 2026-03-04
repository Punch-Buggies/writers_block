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
        // 3.2 Display ok button
        okButton.gameObject.SetActive(true);

        // 4. turn on view
        PurchaseUIView.SetActive(true);
    }

    

}