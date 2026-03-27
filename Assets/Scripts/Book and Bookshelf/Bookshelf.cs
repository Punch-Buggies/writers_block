using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;



// Bookshelf Manager, this just holds the list of books and adds them in when they published
public class BookshelfManager : MonoBehaviour
{
    public static BookshelfManager Instance {get; private set;}
    public List<Book> publishedBooks = new List<Book>();

    private void Awake()
    {
        // if it exists but its not this, destroy
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
           Debug.Log("ahhh the bookshelf is being destroyed");
           Destroy(gameObject);
           return; 
        }
    }

    public void addBook(Book book)
    {
        Debug.Log("want to add a book");
        publishedBooks.Add(book);
        Debug.Log("added a book?");
    }

    public int getBookCount()
    {
        return publishedBooks.Count;
    }

}