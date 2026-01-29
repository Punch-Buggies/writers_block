using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IDropHandler
{

    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject publishableTile;

    [SerializeField] string tileType;

    Slider progressBarSlider;
    StoryElementSupplier storyElementSupplier;
    float timer = 0f;
    [SerializeField] float timerLimit = 2f;

    bool tileOccupied = false;


    bool startCooking = false;
    GameObject spawnedElement;

    void Awake()
    {
        storyElementSupplier = FindAnyObjectByType<StoryElementSupplier>();
        progressBar.SetActive(false);
        progressBarSlider = progressBar.GetComponent<Slider>();
    }
    
    public void OnDrop(PointerEventData eventData)
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


            progressBar.SetActive(false);
            timer = 0f;

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
}
