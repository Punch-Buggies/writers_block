using System.Collections.Generic;
// using System.Diagnostics;
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
            Debug.Log("element count before full pub" + bookStoryElementDict.Count);
            FullPublish();
            Debug.Log("element count after full pub" + bookStoryElementDict.Count);
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
            publishCounter = 0; // checking how many categories of elements we have (genre, char, setting)


            // increasing money because we sold something
            Debug.Log($"Before: ${MoneyManager.Instance.currentMoney}");
            MoneyManager.Instance.addMoney(200);
            Debug.Log($"Published! ${MoneyManager.Instance.currentMoney}");

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
