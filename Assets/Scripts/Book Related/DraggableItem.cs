using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject BGImage;
    [SerializeField] Transform initialLocation;
    bool wasPlacedSuccessfully = false;
    private CanvasGroup canvasGroup;

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
        BGImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BGImage.SetActive(false);
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
        BGImage.SetActive(false);
    }
}
