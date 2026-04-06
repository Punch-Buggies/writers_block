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
            // first check if they've published one book
            if (BookshelfManager.Instance.getBookCount() == 0)
            {
                AudioManager.Instance.PlaySFX("click");
                // Display You cannot erase
                string item = "eraser";
                PurchaseManager.Instance.DisplayNoBooksPublished(item);
            }
            else // feel free to erase
            {
                AudioManager.Instance.PlaySFX("eraser");
                Destroy(gameObject);
            }
            

            // if the publishable tile was in the publish zone
            // erase it from the dictionary

            return;
        }

    }
}
