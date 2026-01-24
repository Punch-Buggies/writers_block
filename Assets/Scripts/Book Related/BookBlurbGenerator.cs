using UnityEngine;


public class BookBlurbGenerator : MonoBehaviour
{
    [SerializeField] string genre;
    [SerializeField] string character;
    [SerializeField] string setting;

    [SerializeField] BookBlurbSupplier bookBlurbSupplier;
    
    string sample_template = "{0} fell in love with a {1} {2}!";

    void Start()
    {
        var femaleNames = bookBlurbSupplier.GetNames("f");
        int idx = Random.Range(0, femaleNames.Count);
        string charA = femaleNames[idx];
        Debug.Log(string.Format(sample_template, charA, "B", "C"));
    }
    
}
