using UnityEngine;

public class BookInsideSpawner : MonoBehaviour
{
    [SerializeField] private GameObject book_inside_prefab;
    [SerializeField] private Transform parentContainer;



    public void SpawnItem(string blurb)
    {
        GameObject newItem = Instantiate(book_inside_prefab, parentContainer);

        BookInside itemScript = newItem.GetComponent<BookInside>();
        itemScript.Initialize(blurb);
    }

}
