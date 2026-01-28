using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Diagnostics;



// Bookshelf Manager, this just holds the list of books and adds them in when they published
public class Bookshelf : MonoBehaviour
{
    public List<Book> publishedBooks = new List<Book>();

    public void addBook(Books book)
    {
        publishedBooks.Add(book);
    }
}