using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Publish : MonoBehaviour
{
    [SerializeField] int publishCounter = 0;

    Dictionary<string, string> bookStoryElementDict;

    List<GameObject> publishedTiles;


    void Awake()
    {
        bookStoryElementDict = new Dictionary<string, string>();
        publishedTiles = new List<GameObject>();
    }

    void Update()
    {
        if(publishCounter >= 3)
        {
            Debug.Log(bookStoryElementDict.Count);
            FullPublish();
            Debug.Log(bookStoryElementDict.Count);
        }
    }

    void FullPublish()
    {
            Debug.Log("Publish time!");

            Debug.Log("Published the book with: ");
            foreach(string key in bookStoryElementDict.Keys)
            {
                Debug.Log(key  + " and " + bookStoryElementDict[key]);
            }

            foreach(GameObject publishedTile in publishedTiles)
            {
                Destroy(publishedTile);
            }

            bookStoryElementDict.Clear();
            publishedTiles.Clear();
            publishCounter = 0;       
    }

    public void PublishStoryElement(string storyElement, string elementType)
    {
        publishCounter += 1;
        Debug.Log("Published Story Element: " +  storyElement + " with " + elementType);
        bookStoryElementDict.Add(storyElement, elementType);
    }

    public Dictionary<string, string> GetBookStoryElementDict()
    {
        return bookStoryElementDict;
    }

    public void AddToPublishedTiles(GameObject publishedTile)
    {
        publishedTiles.Add(publishedTile);
    }

}
