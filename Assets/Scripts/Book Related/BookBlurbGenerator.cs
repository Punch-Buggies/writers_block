using UnityEngine;
using System.Collections.Generic;

public class BookBlurbGenerator : MonoBehaviour
{
    // genre, character, & setting will come from published book
    [SerializeField] string genre;
    [SerializeField] string character;
    [SerializeField] string setting;

    [SerializeField] BookBlurbSupplier bookBlurbSupplier;//gives associated words
    

    void generate_sample()
    {   
        // choose template
        var possible_templates = bookBlurbSupplier.GetTemplates(genre);
        BookBlurbTemplate sample_template = possible_templates[Random.Range(0, possible_templates.Count)];

        Debug.Log(sample_template.baseText);

        // replace variables
        int n = sample_template.slots.Count;
        chosen_words = List<string>;
        foreach (int i in n)
        {
            chosen_words.Add();
        }

        var femaleNames = bookBlurbSupplier.GetNames("f");
        int id1 = Random.Range(0, femaleNames.Count);
        string charA = femaleNames[id1];

        var people = bookBlurbSupplier.GetPeople(setting);
        int id2 = Random.Range(0, people.Count);
        string charB = people[id2];

        var adjectives = bookBlurbSupplier.GetAdjectives(character);
        int id5 = Random.Range(0, adjectives.Count);
        string adj = adjectives[id5];

        // Debug.Log($"{charA} fell in love with {(startsVowel(adj)?"an":"a")} {adj} {charB}!");

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
