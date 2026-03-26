using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PublishableTile : MonoBehaviour, IDropHandler
{
    [SerializeField] string storyElement;
    [SerializeField] string elementType;
    TextMeshProUGUI elementText;

    void Awake()
    {
        elementText = GetComponentInChildren<TextMeshProUGUI>();
    }


    public void SetStoryElement(string element)
    {
        storyElement = element;
    }

    public void SetElementType(string type)
    {
        elementType = type;
        elementText.text = type;
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
            AudioManager.Instance.PlaySFX("eraser");
            Destroy(gameObject);

            // if the publishable tile was in the publish zone
            // erase it from the dictionary

            return;
        }

    }
}
