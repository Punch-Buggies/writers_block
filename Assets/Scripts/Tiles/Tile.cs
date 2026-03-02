using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject publishableTile;

    [SerializeField] GameObject genrePublishableTile;
    [SerializeField] GameObject settingsPublishableTile;
    [SerializeField] GameObject characterPublishableTile;

    [SerializeField] string tileType;
    [SerializeField] TextMeshProUGUI unlockText;

    Slider progressBarSlider;
    StoryElementSupplier storyElementSupplier;
    float timer = 0f;
    [SerializeField] float timerLimit = 2f;

    bool tileOccupied = false;


    bool startCooking = false;
    GameObject spawnedElement;
    Image image;


    int unlockCost = 50;
    [SerializeField] bool unlocked = false;

    Color genreLock = new Color32(101, 160, 189, 255);
    Color settingsLock = new Color32(61, 113, 55, 255);
    Color characterLock = new Color32(201, 188, 99, 255);

    Color genreUnlock = new Color32(62, 169, 244, 181);
    Color settingsUnlock = new Color32(61, 152, 64, 204);
    Color characterUnlock = new Color32(239, 246, 32, 192);


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
            unlockText.rectTransform.anchoredPosition += new Vector2(0, 9);
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if(tileOccupied == true && eventData.pointerDrag.GetComponent<Eraser>() != null)
        {
            Debug.Log("Erasing");
            Destroy(spawnedElement);
            spawnedElement = null;
            tileOccupied = false;
            startCooking = false;
            timer = 0f;
            progressBar.SetActive(false);
            return;
        }
        
        // if its not an eraser and the story element matches the tile type
        if (unlocked && eventData.pointerDrag.GetComponent<Eraser>() == null && eventData.pointerDrag.GetComponent<StoryElement>().GetStoryElement() == tileType)
        {
            spawnedElement = eventData.pointerDrag;
            DraggableItem draggable = spawnedElement.GetComponent<DraggableItem>();
            StoryElement storyElement = spawnedElement.GetComponent<StoryElement>();

            if (tileOccupied == false && storyElement != null)
            {
                progressBar.SetActive(true);
                AudioManager.Instance.PlayQuillSFX();
                if (draggable != null)
                {
                    // StoryElement Side of things
                    draggable.OnSuccessfulDrop(transform.position);
                    storyElement.OnSuccessfulDrop();

                    tileOccupied = true;
                    startCooking = true;
                }   
            }
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
        if(timer <= timerLimit)
        {
            timer += Time.deltaTime;
            progressBarSlider.value = timer;        
        }
        else
        {
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
            unlockText.text = $"{tileType}\n Unlock: " + unlockCost + "$";
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
        if(unlocked == false && MoneyManager.Instance.getMoney() >= unlockCost)
        {
            MoneyManager.Instance.deductMoney(unlockCost);
            
            unlocked = true;

            // changing color now that it is unlocked
            changeColor();
            unlockText.text = tileType;
            // taking away the offset when it gets unlocked bc there is only one line of text now
            unlockText.rectTransform.anchoredPosition -= new Vector2(0, 79);
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
}
