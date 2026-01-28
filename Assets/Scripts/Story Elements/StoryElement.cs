using System;
using TMPro;
using UnityEngine;

public class StoryElement : MonoBehaviour
{
    [SerializeField] string storyElement; // this is the type (genre, char, setting)

    [SerializeField] string elementType; // this is value e.g. action or fantasy

    TextMeshProUGUI elementTypeText;


    void Start()
    {
        elementTypeText = GetComponentInChildren<TextMeshProUGUI>();
        elementTypeText.text = elementType;
    }

    public void SetStoryElement(string element)
    {
        storyElement = element;       
    }

    public string GetStoryElement()
    {
        return storyElement;
    }
    public void SetElementType(string type)
    {
        elementType = type;        
    }
    public string GetElementType()
    {
        return elementType;
    }
}
