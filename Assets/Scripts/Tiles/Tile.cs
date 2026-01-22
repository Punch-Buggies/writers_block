using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IDropHandler
{

    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject publishableTile;

    Slider progressBarSlider;
    StoryElementSupplier storyElementSupplier;
    float timer = 0f;
    float timerLimit = 10f;

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
        Debug.Log("Haha!!!");      
        spawnedElement = eventData.pointerDrag;
        DraggableItem draggable = spawnedElement.GetComponent<DraggableItem>();
        StoryElement storyElement = spawnedElement.GetComponent<StoryElement>();

        if (tileOccupied == false && storyElement != null)
        {
             progressBar.SetActive(true);
            if (draggable != null)
            {
                draggable.OnSuccessfulDrop(transform.position);
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
            Destroy(spawnedElement );
            GameObject spawnedTile = Instantiate(
                publishableTile,
                transform.position,
                Quaternion.identity,
                transform.parent // Set as child of the parent GameObject
            );
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
        Debug.Log(timer);       
    }
}
