using UnityEngine;

public class BookInsideSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bookInsidePrefab;
    [SerializeField] private GameObject bookCoverPrefab;
    [SerializeField] private Transform parentContainer;



    public void SpawnInside(string blurb )
    {
        // spawn inside
        GameObject newInside = Instantiate(bookInsidePrefab, parentContainer);

        BookInside insideScript = newInside.GetComponent<BookInside>();
        insideScript.Initialize(blurb);


    }

    public void SpawnCover(string title, bool bestseller, int copiesSold)
    {
        // spawn outside
        GameObject newCover = Instantiate(bookCoverPrefab, parentContainer);
        BookCover coverScript = newCover.GetComponent<BookCover>();
        string bestsellerStr = bestseller ? "Bestseller" : "";;
        string coverHeaderStr = $"{copiesSold}\ncopies sold\n{bestsellerStr}";
        coverScript.Initialize(title, coverHeaderStr);
    }

}
