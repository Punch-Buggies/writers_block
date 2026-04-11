using System.Collections.Generic;
using System.ComponentModel;

// using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;

public class Publish : MonoBehaviour
{
    [SerializeField] BestSeller bestSeller;
    [SerializeField] public int publishCounter = 0;
    [SerializeField] BookInsideSpawner spawner;
    PassiveIncomeManager passiveIncomeManager;
    [SerializeField] Image bookshelfImage;
    [SerializeField] Image publishButton;
    [SerializeField] Image darkBGPublish;


    [SerializeField] GameObject publishedBookFloatingText;
    [SerializeField] float publishFloatDistance = 2f;
    [SerializeField] float publishFloatDuration = 1.2f;

    Color32 flashColor = new Color32(197, 120, 83, 255);
    Color originalColor = Color.white;
    
    Color publishBorderColor;
    Color publishTextColor;

    Dictionary<string, string> bookStoryElementDict; // keeps track of what elements are sitting in the ui currently

    List<GameObject> publishedTiles;
    int bestSellerMultiplier;


    public Sprite publishReadyButtonImage;
    public Sprite publishNotReadyButtonImage;
    public Button publishUIButton;


    public ParticleSystem[] particleSystem;
    void Awake()
    {
        bookStoryElementDict = new Dictionary<string, string>();
        publishedTiles = new List<GameObject>();
        passiveIncomeManager = FindAnyObjectByType<PassiveIncomeManager>();
        
        publishedTiles = new List<GameObject>();   
    }
    void Start()
    {
        if (publishButton != null)
        {
            publishBorderColor = publishButton.color;
            publishTextColor = publishButton.GetComponentInChildren<TextMeshProUGUI>().color;
        }

        foreach(ParticleSystem ps in particleSystem)
        {
            ps.Stop();
        }


        TextMeshProUGUI tmp = publishedBookFloatingText.GetComponent<TextMeshProUGUI>();
        Color c = tmp.color;
        c.a = 0f;
        tmp.color = c;
    }
    public bool isInDictionary(string tileType)
    {
        bool inDict = bookStoryElementDict.ContainsKey(tileType);
        Debug.Log($"checking the dictionary it says {inDict}  " + string.Join(", ", bookStoryElementDict.Keys));
        return inDict;
    }

    public void PublishButtonClicked()
    {        
        // toggle info is handled in the publish highlight script
        // this is purely for publishing
        if(publishCounter >= 3)
        {
            Debug.Log("element count before full pub" + bookStoryElementDict.Count);
            FullPublish();
            // check if this is the first book you've published
            if (BookshelfManager.Instance.getBookCount() == 1)
            {
                // display go check out the bookshelf, if its the first time!!
                PurchaseManager.Instance.DisplayNoBooksPublished("first book"); // poorly labeled function name, its jsut the tutorial display

            }
            Debug.Log("element count after full pub" + bookStoryElementDict.Count);

            foreach(ParticleSystem ps in particleSystem)
            {
                ps.Play();
            }
        }
        // switch back to publish not ready border sprite
        if (publishButton.sprite != publishNotReadyButtonImage)
        {
            publishButton.sprite = publishNotReadyButtonImage;
            // set to right color
            publishButton.color = publishBorderColor;
            TextMeshProUGUI text = publishButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color = publishTextColor;
            // turn off text glow
            Material mat = text.fontMaterial;
            mat.DisableKeyword("GLOW_ON");
            Debug.LogWarning($"Text is? {text} and is color {text.color}");
        }

    }
    void Update()
    {
        // flash the publish ready sprite if something can be publihsed
        if (publishCounter == 3)
        {
            // switch the button sprite
            publishButton.sprite = publishReadyButtonImage;
            // make the image red
            publishButton.color = Color.red;
            // get the text 
            TextMeshProUGUI text = publishButton.GetComponentInChildren<TextMeshProUGUI>();
            // make the text white
            text.color = Color.white;
            // turn on text glow
            Material mat = text.fontMaterial;
            // make glow glow
            float glow = Mathf.PingPong(Time.time * 1f, 1.2f); //pulse calculation
            mat.SetFloat("_GlowPower", glow);
            mat.EnableKeyword("GLOW_ON");
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

        Debug.Log(BookshelfManager.Instance);
        Debug.Log("i want to add a book bro");
        // add new book data object to the bookshelf
        BookshelfManager.Instance.addBook(newBook);

        // flash bookshelf or replace with book opening animation
        StartCoroutine(Flash(bookshelfImage));
        // everything should have reset clear the best seller highlight
        ClearBSMatch();

        // display floating text for published book and money earned
        TextMeshProUGUI tmp = publishedBookFloatingText.GetComponent<TextMeshProUGUI>();
        tmp.text = "Published: " + title + "\nMoney Earned: $" + bookProfit;
        StartCoroutine(AnimatePublishText(tmp));


    }

    IEnumerator AnimatePublishText(TextMeshProUGUI tmp)
    {
        // Kill any existing tweens
        tmp.DOKill();

        // Reset
        Vector3 startPos = publishedBookFloatingText.transform.position;
        Color c = tmp.color;
        c.a = 0f;
        tmp.color = c;

        Sequence seq = DOTween.Sequence();

        // Float up
        seq.Append(publishedBookFloatingText.transform
            .DOMove(startPos + Vector3.up * publishFloatDistance, publishFloatDuration)
            .SetEase(Ease.OutCubic));

        // Fade in
        seq.Insert(0f, DOTween
            .To(() => tmp.color.a, a => { c = tmp.color; c.a = a; tmp.color = c; }, 1f, publishFloatDuration * 0.3f)
            .SetEase(Ease.OutQuad));

        // Fade out
        seq.Insert(publishFloatDuration * 0.5f, DOTween
            .To(() => tmp.color.a, a => { c = tmp.color; c.a = a; tmp.color = c; }, 0f, publishFloatDuration * 0.5f)
            .SetEase(Ease.InQuad));

        yield return seq.WaitForCompletion();

        // Reset position for next time
        publishedBookFloatingText.transform.position = startPos;
    }

    public void PublishStoryElement(string storyElement, string elementType)
    {
        // adds whatever values into the dictionary (genre, comedy)
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

    private IEnumerator Flash(Image objectToFlash)
    // i tried to make this modular for the pbulish button and bookshelf but it was kinda hard
    // so its really just for the bookshelf
    {
        int flashCount = 3;
        float flashDuration = 0.4f; 

        for (int i = 0; i < flashCount; i++)
        {
            // set to bright color
            objectToFlash.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // return to original color
            objectToFlash.color = originalColor; // orignal color is white, this is for the bookshelf
            yield return new WaitForSeconds(flashDuration);
        }
    }
}
