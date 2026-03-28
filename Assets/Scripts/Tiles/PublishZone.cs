using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PublishZone : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameObject publishedElement; // the element that gets dropped in
    Publish publish;

    [SerializeField] string zoneType;

    bool occupied = false;
    Color ogColor;

    void Awake()
    {
        publish = FindAnyObjectByType<Publish>();
    }
    void Start()
    {
        ogColor = GetComponent<Image>().color;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("click");
        // display info data if clicked on AND toggleINFO on
        if (PurchaseManager.Instance.toggleOnInfo == true)
        {
            // display a information text saying what it does
            string text = $"This is a {zoneType} publish zone, the {zoneType} tile dropped in here will be used to write your next book.";
            PurchaseManager.Instance.DisplayTileInfo(text);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PurchaseManager.Instance.toggleOnInfo == true)
        {
            // darken the image so people know you can click on it
            GetComponent<Image>().color = ogColor * 0.5f;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = ogColor;
    }

}
