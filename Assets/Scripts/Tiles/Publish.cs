using System.Collections.Generic;
using System.ComponentModel;

// using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using UnityEngine;

public class Publish : MonoBehaviour
{
    [SerializeField] int publishCounter = 0;

    Dictionary<string, string> bookStoryElementDict; // keeps track of what elements are sitting in the ui currently

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

    bool bestSelling()
    // this function determines whether the current book has ALL the best selling requirements
    {
        // always returns false for noe
        return false;
    }

    public (int, int) calculateCopiesAndMoney()
    /*
    This function calculates how many copies are sold based on whether it matches the best selling categories, then calculates how much profit the author gets.
    Returns copiesSold and bookProfit
    */ 
    {
        // set at 200 for now but should be based on best selling requirements
        int copiesSold = 300;
        // set at x2 multiplier but will also change based on money function
        int bookProfit = copiesSold * 2;

        return (copiesSold, bookProfit);
    }

    void FullPublish()
    {
            Debug.Log("Publish time!");
            // print the elements in the book
            Debug.Log("Published the book with: ");
            foreach(string key in bookStoryElementDict.Keys)
            {
                Debug.Log(key  + " and " + bookStoryElementDict[key]);
            }
            // delete the tiles in the ui
            foreach(GameObject publishedTile in publishedTiles)
            {
                Destroy(publishedTile);
            }

            // calculate copy and profit info needed for book and then make the book
            (int copiesSold, int bookProfit) = calculateCopiesAndMoney();
            // when a new book is instantiated the money is added INSIDE the bookclass instatntiation
            Book newBook = new Book(
                bookStoryElementDict["Genre"],
                bookStoryElementDict["Character"],
                bookStoryElementDict["Setting"],
                copiesSold,
                bookProfit,
                false
                );
            newBook.blurb = BookBlurbGenerator.Instance.generate_sample("Romance", "Villian", "Castle");


            // clear the element dictionary
            bookStoryElementDict.Clear();
            publishedTiles.Clear();
            publishCounter = 0; // checking how many categories of elements we have (genre, char, setting)            

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
