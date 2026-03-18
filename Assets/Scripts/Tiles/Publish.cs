using System.Collections.Generic;
using System.ComponentModel;

// using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

public class Publish : MonoBehaviour
{
    [SerializeField] BestSeller bestSeller;
    [SerializeField] int publishCounter = 0;
    [SerializeField] BookInsideSpawner spawner;
    PassiveIncomeManager passiveIncomeManager;
    [SerializeField] Image bookshelfImage;
    Color originalColor;

    Dictionary<string, string> bookStoryElementDict; // keeps track of what elements are sitting in the ui currently

    List<GameObject> publishedTiles;
    int bestSellerMultiplier;
    void Awake()
    {
        bookStoryElementDict = new Dictionary<string, string>();
        publishedTiles = new List<GameObject>();
        passiveIncomeManager = FindAnyObjectByType<PassiveIncomeManager>();
        
        publishedTiles = new List<GameObject>();   
    }
    void Start()
    {
        if (bookshelfImage != null)
        {
            originalColor = bookshelfImage.color;
        }
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

    public (int, int) calculateCopiesAndMoney()
    /*
    This function calculates how many copies are sold based on whether it matches the best selling categories, then calculates how much profit the author gets.
    Returns copiesSold and bookProfit
    */ 
    {
        // set at 200 for now but should be based on best selling requirements
        int copiesSold = 300;
        // set at x2 multiplier but will also change based on money function
        int bookProfit = copiesSold * (bestSellerMultiplier + 1);

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

        bestSellerMultiplier = bestSeller.BestSellerMultiplicationCalc(bookStoryElementDict);
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

        newBook.blurb = BookBlurbGenerator.Instance.generate_blurb(newBook.genre, newBook.character, newBook.setting);


        // clear the element dictionary
        bookStoryElementDict.Clear();
        publishedTiles.Clear();
        publishCounter = 0; // checking how many categories of elements we have (genre, char, setting)    

        //display book ui - temporary
        string title = BookBlurbGenerator.Instance.generate_blurb("Title", newBook.character, newBook.setting);
        // makes bookui object and adds to bookshelf UI
        // spawner.SpawnCover(title, newBook.bestSelling, newBook.copiesSold, newBook.blurb);

        passiveIncomeManager.AddBookToPassiveIncome(title, newBook); // This here is used for Passive income, We can keep track of the title and best seller matching
        spawner.SpawnCover(title, newBook);

        // play book sound!!
        AudioManager.Instance.PlayUniqueBookSound(newBook.genre, newBook.character, newBook.setting);

        // flash bookshelf or replace with book opening animation
        StartCoroutine(FlashBookshelf());
        // everything should have reset clear the best seller highlight
        ClearBSMatch();
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
    void ClearBSMatch()
    {
        // only called after a book is publish, there is nothing left in publish so best seller should always clear
        // otherwise turn off the glow
        bestSeller.MaterialMatchGlow("Genre", false);
        bestSeller.MaterialMatchGlow("Character", false);
        bestSeller.MaterialMatchGlow("Setting", false);
    }

    public void CheckBSMatch()
    {
        // if the elementType matches a best seller make it glow on the board
        // first bestSeller is the bestseller script
        // second bestseller (.bestseller) is the list of current best sellers
        foreach (KeyValuePair<string, string> kvp in bookStoryElementDict)
        {
            bool match = false;
            // if it matches light it up
            if (bestSeller.bestSeller.Contains(kvp.Value) )
            {
                Debug.Log($"WE got a match!!! {kvp.Value} was found in {bestSeller.bestSeller}");
                // we got a match yipee
                match = true;
            }
            // otherwise turn off the glow
            bestSeller.MaterialMatchGlow(kvp.Key, match);
        }
        Debug.Log("Done checking if the best seller matches in publish.cs");
    }

    private IEnumerator FlashBookshelf()
    {
        int flashCount = 3;
        Color32 flashColor = new Color32(191, 158, 116, 255);
        float flashDuration = 0.4f; 

        for (int i = 0; i < flashCount; i++)
        {
            // set to bright color
            bookshelfImage.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // return to original color
            bookshelfImage.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

}
