using UnityEngine;

public class ThoughtBubbleHandler : MonoBehaviour
{
    [SerializeField] Transform[] elementSpawnLocations;
    [SerializeField] GameObject storyElementPrefab;

    [SerializeField] string storyElement;
    string[] elements;

    StoryElementSupplier storyElementSupplier;


    void Awake()
    {
        storyElementSupplier = FindAnyObjectByType<StoryElementSupplier>();
        GetLocationTransforms();
        GetStoryElements();   
    }

    void Start()
    {
        SpawnStoryElements();
    }

    void SpawnStoryElements()
    {
        int randomElementNumber = Random.Range(0, elements.Length);
        int spawnCount = Mathf.Min(elements.Length, elementSpawnLocations.Length); // Make sure we don't spawn more elements than we have locations
        
        for (int i = 0; i < spawnCount; i++)
        {
            randomElementNumber = Random.Range(0, elements.Length);
            GameObject spawnedElement = Instantiate(
                storyElementPrefab, 
                elementSpawnLocations[i].position, 
                Quaternion.identity, 
                transform // Set as child of this GameObject
            );
            

            // Filling up the data for both the scripts
            StoryElement storyElementComponent = spawnedElement.GetComponent<StoryElement>();
            if (storyElementComponent != null)
            {
                storyElementComponent.SetStoryElement(storyElement);
                storyElementComponent.SetElementType(elements[randomElementNumber]);
            }
            
            DraggableItem draggable = spawnedElement.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                draggable.SetInitialLocation(elementSpawnLocations[i]);
            }
        }
    }

    void GetStoryElements()
    {
        switch (storyElement)
        {
            case "Genre":
                elements = storyElementSupplier.GetGenreTypes();
                break;
            case "Character":
                elements = storyElementSupplier.GetCharacterTypes();
                break;
            case "Setting":
                elements = storyElementSupplier.GetSettingTypes();
                break;
            default:
                Debug.Log("No Story Element Defined");
                break;
        }       
    }

    void GetLocationTransforms() // Gets all the Spawn Locations
    {
        int childCount = transform.childCount;
        
        int locationCount = 0;
        for (int i = 0; i < childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("Location"))
            {
                locationCount++;
            }
        }
        
        elementSpawnLocations = new Transform[locationCount];
        
        int index = 0;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Location"))
            {
                elementSpawnLocations[index] = child;
                index++;
            }
        }        
    }
}
