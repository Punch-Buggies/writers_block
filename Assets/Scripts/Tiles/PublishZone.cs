using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PublishZone : MonoBehaviour, IDropHandler
{
    GameObject publishedElement;
    Publish publish;

    [SerializeField] string zoneType;

    bool occupied = false;

    void Awake()
    {
        publish = FindAnyObjectByType<Publish>();
    }
    public void OnDrop(PointerEventData eventData)
    {

        publishedElement = eventData.pointerDrag;
        DraggableItem draggable = publishedElement.GetComponent<DraggableItem>();
        PublishableTile publishableTile = publishedElement.GetComponent<PublishableTile>();


        if (publishableTile != null && publishableTile.GetStoryElement() == zoneType)
        {
            if (draggable != null)
            {
                AudioManager.Instance.PlaySFX("drop");
                draggable.OnSuccessfulDrop(transform.position);
                string storyElement = publishableTile.GetStoryElement(); //char, genre, setting
                string elementType = publishableTile.GetElementType(); //value
                // adds the stuff into the dictionary
                publish.PublishStoryElement(storyElement, elementType);
                publish.AddToPublishedTiles(publishedElement);

                // we're adding soemthing to publish check best seller
                publish.CheckBSMatch();

            }
        }

    }

}
