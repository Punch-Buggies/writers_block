using UnityEngine;

public class BookInsideSpawner : MonoBehaviour
{
    // [SerializeField] private GameObject bookInsidePrefab;
    [SerializeField] private GameObject bookCoverPrefab;
    [SerializeField] private Transform parentContainer;

    // [SerializeField] private Transform bookViewContainer;
    

    // public void SpawnInside(string blurb )
    // {
    //     // spawn inside
    //     GameObject newInside = Instantiate(bookInsidePrefab, bookViewContainer);

    //     BookInside insideScript = newInside.GetComponent<BookInside>();
    //     insideScript.Initialize(blurb);

    // }

    public void SpawnCover(string title, bool bestseller, int copiesSold, string blurb)
    {
        // spawn outside
        GameObject newCover = Instantiate(bookCoverPrefab, parentContainer);
        BookCover coverScript = newCover.GetComponent<BookCover>();
        string bestsellerStr = bestseller ? "Bestseller" : "";;
        string coverHeaderStr = $"{copiesSold}\ncopies sold\n{bestsellerStr}";
        coverScript.Initialize(title, coverHeaderStr, blurb);
    }

}
