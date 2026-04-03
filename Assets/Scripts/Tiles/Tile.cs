using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] GameObject nextTileToUnlock;
    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject publishableTile;

    [SerializeField] GameObject genrePublishableTile;
    [SerializeField] GameObject settingsPublishableTile;
    [SerializeField] GameObject characterPublishableTile;

    [SerializeField] string tileType;
    [SerializeField] TextMeshProUGUI unlockText;
    [SerializeField] private TMP_FontAsset font;

    Slider progressBarSlider;
    StoryElementSupplier storyElementSupplier;
    float timer = 0f;
    [SerializeField] float timerLimit = 10f;

    bool tileOccupied = false;


    bool startCooking = false;
    GameObject spawnedElement;
    Image image;


    double unlockCost;
    double growthCost;
    [SerializeField] bool unlocked = false;

    Color genreLock = new Color32(101, 160, 189, 255);
    Color settingsLock = new Color32(61, 113, 55, 255);
    Color characterLock = new Color32(201, 188, 99, 255);

    Color genreUnlock = new Color32(62, 169, 244, 181);
    Color settingsUnlock = new Color32(61, 152, 64, 204);
    Color characterUnlock = new Color32(239, 246, 32, 192);

    bool playGrowSound = true;

    void Awake()
    {
        storyElementSupplier = FindAnyObjectByType<StoryElementSupplier>();
        unlockText.text = "";
        progressBar.SetActive(false);
        progressBarSlider = progressBar.GetComponent<Slider>();

        image = GetComponent<Image>();

        // change image color
        changeColor();
        if (!unlocked) // in lock position
        {
           // offsetting the text because in the lock position there is two lines
            // unlockText.rectTransform.anchoredPosition += new Vector2(0, 11); 
        }
    }

    void Start()
    {
        // it seems like tile.awake is called before moneymanager.awake so having this in awake, moneymanager doesnt exist yet
        unlockCost = MoneyManager.Instance.tileUnlockCost;
        unlockText.font = font;
    }

    private void Erase(GameObject spawnedElement)
    {
        // THIS NEVER GETS CALLED
        AudioManager.Instance.PlaySFX("eraser");
        Debug.Log("Erasing");
        Destroy(spawnedElement);
        spawnedElement = null;
        tileOccupied = false;
        startCooking = false;
        timer = 0f;
        progressBar.SetActive(false);
        return;
    }

    private void SetUpCook(GameObject spawnedElement)
    {
        DraggableItem draggable = spawnedElement.GetComponent<DraggableItem>();
        StoryElement storyElement = spawnedElement.GetComponent<StoryElement>();

        // start progress bar
        progressBar.SetActive(true);
        // audio
        AudioManager.Instance.PlaySFX("quill");
        if (draggable != null)
        {
            // StoryElement Side of things
            // change position
            draggable.OnSuccessfulDrop(transform.position);
            storyElement.OnSuccessfulDrop();

            tileOccupied = true;
            startCooking = true; // now that this is true, the next update call will run cooking and timering
        }  
    }
    
    // dropping something over a tile
    public void OnDrop(PointerEventData eventData)
    {
        // its in eraser mode // THIS NEVER EXECUTES
        if(tileOccupied == true && eventData.pointerDrag.GetComponent<Eraser>() != null)
        {
            Debug.Log("about to erase");
            Erase(spawnedElement);
            Debug.Log("erase called?");
        }
        
        // if its not an eraser, the story element matches the tile type, the tile is unlocked and not occuiped
        // first the tile must be unlocked, unoccupied and not in eraser mode
        // then check if its a story element(to be grown) or a publishable tile
        
        // if (unlocked && eventData.pointerDrag.GetComponent<Eraser>() == null && eventData.pointerDrag.GetComponent<StoryElement>().GetStoryElement() == tileType && tileOccupied == false)
        if (unlocked && tileOccupied == false && eventData.pointerDrag.GetComponent<Eraser>() == null) 
        {
            // check if story element or publishable
            if (eventData.pointerDrag.GetComponent<StoryElement>() != null && eventData.pointerDrag.GetComponent<StoryElement>().GetStoryElement() == tileType)
            {
                spawnedElement = eventData.pointerDrag;
                growthCost = MoneyManager.Instance.tileGrowthCost;
            
                StoryElement storyElement = spawnedElement.GetComponent<StoryElement>();
                // the tile is not empty and story element is there
                if (storyElement != null)
                {
                    // dont show again is NOT clicked. so the purchase ui comes up
                    // takes a response from the player
                    if (PurchaseManager.Instance.toggleOnPurchase == true)
                    {
                        // confirm with the player that they want to grow this element
                        PurchaseManager.Instance.ConfirmTileGrowthPayment(storyElement, timerLimit, growthCost,confirmed =>
                        {
                            if (confirmed) // they clicked on the yes button  
                            {
                                //process the purchase
                                ProcessPurchase(spawnedElement);
                            }
                            else // they clicked no button
                            {
                                Debug.Log($"Player declined to grow {storyElement.GetElementType()}");
                            }
                        });
                    }
                    else // toggleOnPurchase == false, don't show again was clicked
                    {
                        ProcessPurchase(spawnedElement);
                    } 
                } 
            }
            // // if its a publishanble tile
            // else if (eventData.pointerDrag.GetComponent<PublishableTile>() != null && eventData.pointerDrag.GetComponent<PublishableTile>().GetStoryElement() == tileType)
            // {
            //     // move the tile to that spot
            //     Debug.Log("i knew it was a publishable tile");
            //     DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
            //     // change location
            //     draggable.OnSuccessfulDrop(transform.position);

            //     // need to check if this publishable tile came from the publish zone
            //     // if it did remove it from the dictionary

            // }
        }
    }

    void Update()
    {
        if (startCooking)
        {
            CookingAndTimering();
        }
    }

    void CookingAndTimering()
    {
        // turn off ability to drag tile when cooking
        if(timer <= timerLimit)
        {
            timer += Time.deltaTime;
            progressBarSlider.value = timer;     

            // disable drag
            DraggableItem draggable = spawnedElement.GetComponent<DraggableItem>();   
            draggable.enabled = false;

            // start playing the grow finish audio with 0.8 seconds left
            if (timer >= 9.1f && playGrowSound == true)
            {
                AudioManager.Instance.PlaySFX("grow finish");
                playGrowSound = false;
            }

        }
        else // timer has reached its limit
        {
            playGrowSound = true;
            // Before we destroy the storyElement, we get the info out of it
            string storyElement = spawnedElement.GetComponent<StoryElement>().GetStoryElement();
            string elementType = spawnedElement.GetComponent<StoryElement>().GetElementType();


            GameObject prefabToSpawn = null;
            // we destoy the old element which we dragged
            Destroy(spawnedElement);

            switch (storyElement)
            {
                case "Genre":
                    prefabToSpawn = genrePublishableTile;
                    break;

                case "Setting":
                    prefabToSpawn = settingsPublishableTile;
                    break;

                case "Character":
                    prefabToSpawn = characterPublishableTile;
                    break;

                default:
                    Debug.LogWarning("Unknown element type: " + elementType);
                    return;
            }

            GameObject spawnedTile = Instantiate(
                prefabToSpawn,
                transform.position,
                Quaternion.identity,
                transform
            );

            

            // After Spawning the Publishable Tile, we put the info into it
            spawnedTile.GetComponent<PublishableTile>().SetStoryElement(storyElement);
            spawnedTile.GetComponent<PublishableTile>().SetElementType(elementType);

            spawnedElement = spawnedTile;

            // reset timer, progress bar, boolean
            progressBar.SetActive(false);
            timer = 0f;
            startCooking = false;

            DraggableItem draggable = spawnedTile.GetComponent<DraggableItem>();

            if (draggable != null)
            {
                draggable.SetInitialLocation(transform);
                startCooking = false;
                tileOccupied = false;
            }
        }
        // Debug.Log(timer);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!unlocked)
        {
            // get unlock cost
            unlockCost = MoneyManager.Instance.tileUnlockCost;

            unlockText.text = $"${unlockCost:0.##}";
        }
        else
        {
            unlockText.text = tileType;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unlockText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // unlocking the tile bc you clicked it
        // get unlock cost
        unlockCost = MoneyManager.Instance.tileUnlockCost;

        // first check if they've published one book
        // display no books published
        if (BookshelfManager.Instance.getBookCount() == 0 && unlocked == false)
        {
            AudioManager.Instance.PlaySFX("click");
            // Display You cannot purchase a "" grow tile
            string item = $"{tileType} grow tile";
            PurchaseManager.Instance.DisplayNoBooksPublished(item);
        }
        // display info
        else if (unlocked == true && PurchaseManager.Instance.toggleOnInfo == true)
        {
            AudioManager.Instance.PlaySFX("click");
            // this tile is already unlocked AND they HAVENT TOGGLED THE DISPLAYU OFF
            // display a information text saying what it does
            string text = $"This is a {tileType} grow tile, which can grow {tileType} thoughts when dropped in.";
            PurchaseManager.Instance.DisplayTileInfo(text);
        }
        // buying the tile
        else // they have published at least one book
        {    // tile must be locked
             // and thye have enough money
            if(unlocked == false && MoneyManager.Instance.currentMoney >= unlockCost)
            {
                AudioManager.Instance.PlaySFX("purchase");
                MoneyManager.Instance.deductMoney(unlockCost);
                
                unlocked = true;

                // changing color now that it is unlocked
                changeColor();
                unlockText.text = tileType;
                // taking away the offset when it gets unlocked bc there is only one line of text now
                unlockText.rectTransform.anchoredPosition -= new Vector2(0, 11);

                // changing unlock cost to be 1.5x more now that they have made a purchase
                MoneyManager.Instance.increaseTileCost();

                // show the next tile in the column
                if (nextTileToUnlock != null)
                {
                    nextTileToUnlock.SetActive(true);
                }
            }
            // they didn't have enough money
            else if (unlocked == false && MoneyManager.Instance.currentMoney < unlockCost)
            {
                AudioManager.Instance.PlaySFX("click"); // maybe change this to an insufficeint fund sound
                PurchaseManager.Instance.DisplayInsufficientFunds();
            }
        }
    }

    private void changeColor()
    {
        // checks if its in the unlocked or locked state and sets color accordinly
        if (!unlocked)
        {
            // changing color to type
            switch (tileType)
            {
                case "Genre":
                    image.color = genreLock;
                    break;
                case "Character":
                    image.color = characterLock;
                    break;
                case "Setting":
                    image.color = settingsLock;
                    break;
                default:
                    Debug.LogWarning($"Lock Tile type: {tileType} did not match anything");
                    break;
            }
        }
        else // tile is unlocked
        {
            // changing color to type
            switch (tileType)
            {
                case "Genre":
                    image.color = genreUnlock;
                    break;
                case "Character":
                    image.color = characterUnlock;
                    break;
                case "Setting":
                    image.color = settingsUnlock;
                    break;
                default:
                    Debug.LogWarning($"Unlock Tile type: {tileType} did not match anything");
                    break;
            }
        }
    }

    void ProcessPurchase(GameObject spawnedElement)
    // attempt to process the purchase, if they have insufficient funds tell them
    {
        // first check if they have enough money
        if (MoneyManager.Instance.currentMoney >= growthCost )
        {
            // we had enbough money, deduct, and set up the cook
            MoneyManager.Instance.deductMoney(growthCost);
            SetUpCook(spawnedElement);
        }
        // commenting out the insufficient funds bc we should have gotten rid of this
        else // we didnt have enough money, tell the player
        {
            PurchaseManager.Instance.DisplayInsufficientFunds();
        }
    }

}
