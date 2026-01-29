using UnityEngine;

public class PublishableTile : MonoBehaviour
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
}
