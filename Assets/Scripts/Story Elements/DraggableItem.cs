using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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
    }

    void Start()
    {
        transform.position = initialLocation.position;

        initialScale = transform.localScale;
        finalScale = transform.localScale * scaleFactor;
    }
}
