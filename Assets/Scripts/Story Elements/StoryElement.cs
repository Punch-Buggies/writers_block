using System;
using TMPro;
using UnityEngine;

public class StoryElement : MonoBehaviour
{
    [SerializeField] string storyElement; // this is the type (genre, char, setting)

    [SerializeField] string elementType; // this is value e.g. action or fantasy

    TextMeshProUGUI elementTypeText;
    GameObject parentSpawnLocation;

    


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

    public void SetSpawnParent(GameObject parent)
    {
        parentSpawnLocation = parent;
    }

    public void OnSuccessfulDrop()
    {
        SpawnLocation spawnLocation = parentSpawnLocation.GetComponent<SpawnLocation>();
        spawnLocation.SetOccupation(false);
        Debug.Log("Papa!"); 
    }
}
