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

    public void Initialize(string title, string besteller)
    {

        titleText.text = title;
        bestellerText.text = besteller;
    }


}
