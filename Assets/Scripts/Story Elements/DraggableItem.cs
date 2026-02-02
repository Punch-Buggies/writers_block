using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Transform initialLocation;
    bool wasPlacedSuccessfully = false;
    private CanvasGroup canvasGroup;

    [SerializeField] float scaleFactor = 2f;
    [SerializeField] float scaleTime = 0.5f;

    Vector2 initialScale;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        initialScale = transform.localScale;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; 
        Debug.Log("Begin Drag");
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
            transform.position = initialLocation.position;
            canvasGroup.blocksRaycasts = true; 
        }

        Debug.Log("End Drag");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Uncomment if you don't want to use DOTween and lerping
        // Transform actualImageTransform = transform;
        // Vector2 scaled = new Vector2(actualImageTransform.localScale.x * scaleFactor, 
        //                             actualImageTransform.localScale.y * scaleFactor);

        // actualImageTransform.localScale = scaled;

        transform.DOScale(new Vector3(transform.localScale.x * scaleFactor, 
        transform.localScale.y * scaleFactor, 
        transform.localScale.z), scaleTime).SetEase(Ease.OutBounce);
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
         transform.position = dropPosition;
    }

    void Start()
    {
        transform.position = initialLocation.position;
    }
}
