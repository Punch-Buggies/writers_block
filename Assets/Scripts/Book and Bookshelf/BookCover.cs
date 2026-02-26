using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BookCover : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bestellerText;
    

    private string title;
    private string besteller;
    private string blurb;
    // private Book book;

    public void Initialize(string title, string besteller, string b)
    {

        titleText.text = title;
        bestellerText.text = besteller;
        blurb = b;
    }

    // add button for opening bookview
    public void OnClick(){
        Debug.Log("youre clicking me");
        BookViewManager.Instance.OpenBookView(blurb);
    }

}
