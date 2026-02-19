using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BookInside : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI blurbText;
    private string blurb;

    public void Initialize(string b)
    {
        blurb = b;
        if (blurb != null){blurbText.text = blurb;}
        
    }

    void setBlurb(string b)
    {
        blurb = b;
    }
    public string getBlurb()
    {
        return blurb;
    }
}
