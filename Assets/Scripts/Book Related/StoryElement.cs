using System;
using TMPro;
using UnityEngine;

public class StoryElement : MonoBehaviour
{
    [SerializeField] string storyElement;

    [SerializeField] string elementType;

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
