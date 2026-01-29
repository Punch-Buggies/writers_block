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


        if (publishableTile.GetStoryElement() == zoneType)
        {
            if (draggable != null)
            {
                draggable.OnSuccessfulDrop(transform.position);
                string storyElement = publishableTile.GetStoryElement();
                string elementType = publishableTile.GetElementType();

                publish.PublishStoryElement(storyElement, elementType);
                publish.AddToPublishedTiles(publishedElement);
            }
        }

    }

}
