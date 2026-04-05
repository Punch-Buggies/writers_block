using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Transform initialLocation;
    bool wasPlacedSuccessfully = false;
    private CanvasGroup canvasGroup;

    [SerializeField] float scaleFactor = 2f;
    [SerializeField] float scaleTime = 0.5f;

    Vector2 initialScale;
    Vector2 finalScale;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

    }
    public void DisplayThoughtInfo(StoryElement storyElement)
    {
        // play sound
        AudioManager.Instance.PlaySFX("shortPage");
        string type = storyElement.GetStoryElement();
        string value = storyElement.GetElementType();
        // set a or an depening on first letter of value
        string vowels = "aeiou";
        if (vowels.Contains(value[0]))
        {
            // there is a vowel
            value = "an "+ value;
        }else
        {
            // there is no value
            value = "a " + value;
        }
        // display info
        string text = $"This is {value} {type} thought tile, you can grow this thought only in {type} grow tiles.";
        PurchaseManager.Instance.DisplayTileInfo(text);
    }
    void DisplayEraserInfo()
    {
        if (BookshelfManager.Instance.getBookCount() == 0)
        {
            AudioManager.Instance.PlaySFX("click");
            // Display You cannot erase
            string item = "eraser";
            PurchaseManager.Instance.DisplayNoBooksPublished(item);
        }
        // display erase info
        else if (PurchaseManager.Instance.toggleOnInfo == true)
        {
            AudioManager.Instance.PlaySFX("click");
            // if they HAVENT TOGGLED THE DISPLAY OFF
            // display a information text saying what it does
            string text = $"Drag over any thought in a grow tile to erase it. You cannot erase a thought while it is growing.";
            PurchaseManager.Instance.DisplayTileInfo(text);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // this function deals with display the tile info for story elements or the eraser

        // check if this is a story element or eraser, could also be a publishable tile technically but only check these two cases
        StoryElement storyElement = GetComponent<StoryElement>();
        Eraser eraser = GetComponent<Eraser>();

        // show an info display if the drggable item is also a story element AND check that toggleINFO is on
        if (storyElement != null && PurchaseManager.Instance.toggleOnInfo == true)
        {
            DisplayThoughtInfo(storyElement);
        }
        else if (eraser != null) // dont need to check toggle, thats done in displayeraser info
        {
            DisplayEraserInfo();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {

        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos);
        transform.position = globalMousePos;
        //transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!wasPlacedSuccessfully)
        {
            // if it wasn't placed succesfully return to previous location
            transform.position = initialLocation.position;
            // allow it to be grabbed
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Uncomment if you don't want to use DOTween and lerping
        // Transform actualImageTransform = transform;
        // Vector2 scaled = new Vector2(actualImageTransform.localScale.x * scaleFactor, 
        //                             actualImageTransform.localScale.y * scaleFactor);

        // actualImageTransform.localScale = scaled;

        transform.DOScale(finalScale, scaleTime).SetEase(Ease.OutBounce);

        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(initialScale, scaleTime).SetDelay(scaleTime/2).SetEase(Ease.OutBounce);
    }

    public void SetInitialLocation(Transform location)
    {
        initialLocation = location;
        transform.position = location.position;
    }

    public void OnSuccessfulDrop(Vector3 dropPosition)
    {
        wasPlacedSuccessfully = true;
        // physucally moving it to that position
        transform.position = dropPosition;
        // Debug.Log("how are canvas raycasts after successful drop");
        // Debug.Log(canvasGroup.blocksRaycasts);
        // canvasGroup.blocksRaycasts = true; // can grab after placing in publishable zone but it can now drop anywhere
    }

    void Start()
    {
        transform.position = initialLocation.position;

        initialScale = transform.localScale;
        finalScale = transform.localScale * scaleFactor;
    }
}
