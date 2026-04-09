using UnityEngine;

public class BookInsideSpawner : MonoBehaviour
{
    // [SerializeField] private GameObject bookInsidePrefab;
    [SerializeField] private GameObject bookCoverPrefab;
    [SerializeField] private Transform parentContainer;

   

    public void SpawnCover(string title, Book book)
    {
        int bestseller = book.bestSelling;
        int copiesSold = book.copiesSold;
        // string blurb = book.blurb;
       

        // spawn outside
        GameObject newCover = Instantiate(bookCoverPrefab, parentContainer);
        BookCover coverScript = newCover.GetComponent<BookCover>();
        string bestsellerStr = "";
        if (bestseller > 0)
        {
            bestsellerStr = "Bestseller";
        }
        string coverHeaderStr = $"{copiesSold}\ncopies sold\n{bestsellerStr}";
        Debug.Log("about to initialize cover script this is corner string: "+coverHeaderStr);
        coverScript.Initialize(title, coverHeaderStr, book);
    }

}
