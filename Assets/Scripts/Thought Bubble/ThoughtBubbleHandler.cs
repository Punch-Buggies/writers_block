using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubbleHandler : MonoBehaviour
{
    [SerializeField] GameObject[] elementSpawnLocations;
    [SerializeField] GameObject storyElementPrefab;

    [SerializeField] string storyElement;
    string[] elements;

    StoryElementSupplier storyElementSupplier;


    void Awake()
    {
        storyElementSupplier = FindAnyObjectByType<StoryElementSupplier>();
        GetStoryElements();

        foreach(GameObject loc in elementSpawnLocations)
        {
            // Get the spawn location script
            SpawnLocation spawnLocation = loc.GetComponent<SpawnLocation>();

            // Get the transform
            Transform elementSpawnTransform = spawnLocation.GetTransform();

            // Spawn a story element there
            SpawnStoryElement(elementSpawnTransform, loc);

            // Set it to occupied
            spawnLocation.SetOccupation(true);

        }    
    }

    void Update()
    {
        foreach(GameObject loc in elementSpawnLocations)
        {
            SpawnLocation spawnLocation = loc.GetComponent<SpawnLocation>();
            if (spawnLocation.GetOccupation() == false)
            {
                // Get the transform
                Transform elementSpawnTransform = spawnLocation.GetTransform();

                // Spawn a story element there
                SpawnStoryElement(elementSpawnTransform, loc);

                // Set it to occupied
                spawnLocation.SetOccupation(true);          
            }            
        }   
    } 

    void SpawnStoryElement(Transform elementSpawnTransform, GameObject parent)
    {
        int randomElementNumber = Random.Range(0, elements.Length);
        int spawnCount = Mathf.Min(elements.Length, elementSpawnLocations.Length); // Make sure we don't spawn more elements than we have locations
        
        GameObject spawnedElement = Instantiate(
            storyElementPrefab, 
            elementSpawnTransform.position, 
            Quaternion.identity, 
            transform // Set as child of this GameObject
        );

        UnityEngine.Color c = UnityEngine.Color.blue;
        spawnedElement.GetComponent <Image>().color = c;
        // actual image is whats it called in uniy editor whatevrr man'
        // spawnedElement.GetComponentsInChildren<Image>().color = c;
;
        

        // Filling up the data for both the scripts
        StoryElement storyElementComponent = spawnedElement.GetComponent<StoryElement>();
        if (storyElementComponent != null)
        {
            storyElementComponent.SetStoryElement(storyElement);
            storyElementComponent.SetElementType(elements[randomElementNumber]);
            storyElementComponent.SetSpawnParent(parent);

        }
        
        DraggableItem draggable = spawnedElement.GetComponent<DraggableItem>();
        if (draggable != null)
        {
            draggable.SetInitialLocation(elementSpawnTransform);
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
}
