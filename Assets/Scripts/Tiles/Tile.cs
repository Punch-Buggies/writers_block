using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject publishableTile;

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

    void Awake()
    {
        storyElementSupplier = FindAnyObjectByType<StoryElementSupplier>();
        unlockText.text = "";
        progressBar.SetActive(false);
        progressBarSlider = progressBar.GetComponent<Slider>();

        image = GetComponent<Image>();

        if (unlocked)
        {
            image.color = Color.white;
        }
        else
        {
            image.color = Color.grey;
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
        
        if (unlocked && eventData.pointerDrag.GetComponent<Eraser>() == null)
        {
            spawnedElement = eventData.pointerDrag;
            DraggableItem draggable = spawnedElement.GetComponent<DraggableItem>();
            StoryElement storyElement = spawnedElement.GetComponent<StoryElement>();

            if (tileOccupied == false && storyElement != null)
            {
                progressBar.SetActive(true);
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

            Destroy(spawnedElement);
            GameObject spawnedTile = Instantiate(
                publishableTile,
                transform.position,
                Quaternion.identity,
                transform // Set as child of the parent GameObject
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
            unlockText.text = "Unlock: " + unlockCost + "$";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unlockText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(MoneyManager.Instance.getMoney() >= unlockCost)
        {
            MoneyManager.Instance.deductMoney(unlockCost);
            
            unlocked = true;
            image.color = Color.white;
        }

    }
}
