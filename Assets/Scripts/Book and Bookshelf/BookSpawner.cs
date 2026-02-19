using UnityEngine;

public class BookInsideSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bookInsidePrefab;
    [SerializeField] private GameObject bookCoverPrefab;
    [SerializeField] private Transform parentContainer;



    public void SpawnItem(string blurb, string title, bool bestseller, int copiesSold)
    {
        // spawn inside
        GameObject newInside = Instantiate(bookInsidePrefab, parentContainer);

        BookInside insideScript = newInside.GetComponent<BookInside>();
        insideScript.Initialize(blurb);

        // spawn outside
        GameObject newCover = Instantiate(bookCoverPrefab, parentContainer);
        BookCover coverScript = newCover.GetComponent<BookCover>();
        string bestsellerStr = bestseller ? "Bestselling novel" : "Novel";;
        string coverHeaderStr = $"{bestsellerStr} with {copiesSold} copies sold";
        coverScript.Initialize(title, coverHeaderStr);
    }

}
