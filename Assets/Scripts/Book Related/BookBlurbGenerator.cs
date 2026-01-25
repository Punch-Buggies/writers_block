using UnityEngine;


public class BookBlurbGenerator : MonoBehaviour
{
    [SerializeField] string genre;
    [SerializeField] string character;
    [SerializeField] string setting;

    [SerializeField] BookBlurbSupplier bookBlurbSupplier;
    

    void generate_sample()
    {

        var femaleNames = bookBlurbSupplier.GetNames("f");
        int id1 = Random.Range(0, femaleNames.Count);
        string charA = femaleNames[id1];

        var people = bookBlurbSupplier.GetPeople(setting);
        int id2 = Random.Range(0, people.Count);
        string charB = people[id2];

        var adjectives = bookBlurbSupplier.GetAdjectives(character);
        int id5 = Random.Range(0, adjectives.Count);
        string adj = adjectives[id5];

        Debug.Log($"{charA} fell in love with {(startsVowel(adj)?"an":"a")} {adj} {charB}!");

    }
    
    bool startsVowel(string s)
    // returns true if the first letter of a given string is a vowel
    {
        return "aeiouAEIOU".IndexOf(s[0]) >= 0;
    }

    void Start()
    {
        for (int i = 0; i<5; i++)
        {
            generate_sample();
        }
    }
}
