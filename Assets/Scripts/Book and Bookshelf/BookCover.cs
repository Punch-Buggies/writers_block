using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BookCover : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI cornerText;

    // title to display in prefab
    private string title;
    // corner header string
    private string cornerHeader;
    private Book book;

    public void Initialize(string title, string cornerHeader, Book bookData)
    {

        titleText.text = title;
        cornerText.text = cornerHeader;
        book = bookData;

    }

    // add button for opening bookview
    public void OnClick(){
        Debug.Log("youre clicking me");
        // book sound is also played in thiw function call
        BookViewManager.Instance.OpenBookView(book);
    }

}
