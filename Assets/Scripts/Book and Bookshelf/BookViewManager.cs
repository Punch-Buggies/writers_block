using UnityEngine;

/* This script gets attached to a handler game object, so it can then be attached to the book cover prefab. so that when you click the book object in the bookshelf, the book view appears. */
public class BookViewManager : MonoBehaviour
{
    public static BookViewManager Instance { get; private set;}

    [SerializeField] private GameObject bookViewUI; // the whole thing

    [SerializeField] private Transform bookViewContainer; // just where the prefab is gonna be put
    [SerializeField] private GameObject bookInsidePrefab;

    GameObject currentInsideBook;
    Book currentDataBook;


    private void Awake(){
        // if it exists but its not this, destroy
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
           Debug.Log("ahhh the bookview manager being destroyed");
           Destroy(gameObject);
           return; 
        }
    }

    public void SpawnInside(Book book )
    {
        // spawn inside
        GameObject newInside = Instantiate(bookInsidePrefab, bookViewContainer);

        BookInside insideScript = newInside.GetComponent<BookInside>();
        insideScript.Initialize(book.blurb);

        currentInsideBook = newInside;

        currentDataBook = book;

    }

    public void OpenBookView(Book book)
    {
        // first create the inside view object
        SpawnInside(book);
        // play sound
        AudioManager.Instance.PlayUniqueBookSound(book.genre, book.character, book.setting);
        // then activate the ui overtop the bookshelf
        bookViewUI.SetActive(true); // opens the bookUI
    }

    public void CloseBookView(){
        // play audio
        // AudioManager.Instance.PlayUniqueBookSound(currentDataBook.genre, currentDataBook.character, currentDataBook.setting);
        // first close the UI so now its just the bookshelf showing
        bookViewUI.SetActive(false);
        // then delete game insidne view objecy
        Destroy(currentInsideBook);
        currentDataBook = null;
    }

}