using UnityEngine;
using UnityEngine.EventSystems;

public class PublishableTile : MonoBehaviour, IDropHandler
{
    [SerializeField] string storyElement;
    [SerializeField] string elementType;


    public void SetStoryElement(string element)
    {
        storyElement = element;
    }

    public void SetElementType(string type)
    {
        elementType = type;
    }


    public string GetStoryElement()
    {
        return storyElement;
    }

    public string GetElementType()
    {
        return elementType;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.GetComponent<Eraser>() != null)
        {
            Destroy(gameObject);
            return;
        }
    }
}
