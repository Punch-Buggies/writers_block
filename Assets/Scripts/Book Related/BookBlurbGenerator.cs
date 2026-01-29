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

        // map slotId -> chosen word
        Dictionary<string, string> chosenBySlot = new Dictionary<string, string>();

        // replace variables
        int n = sample_template.slots.Count;
        List<string> chosen_words = new List<string>();
        foreach (var slot in sample_template.slots)
        {
            if (!chosenBySlot.ContainsKey(slot.slotId))//don't overwrite already chosen words
            {
                string word = "";
                switch (slot.type)
                {
                    case WordType.Adjective:
                        word = RandomFrom(bookBlurbSupplier.GetAdjectives(character));
                        break;
                    case WordType.Catchphrase:
                        word = RandomFrom(bookBlurbSupplier.GetCatchphrases(character));
                        break;
                    case WordType.Name:
                        word = RandomFrom(bookBlurbSupplier.GetNames(RandomFrom(new List<string> {"f","m","nb"})));
                        break;
                    case WordType.Person:
                        word = RandomFrom(bookBlurbSupplier.GetPeople(setting));
                        break;
                    case WordType.Place:
                        word = RandomFrom(bookBlurbSupplier.GetPlaces(setting));
                        break;
                    case WordType.Thing:
                        word = RandomFrom(bookBlurbSupplier.GetThings(setting));
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(slot.type), slot.type, null);
                }

                chosenBySlot[slot.slotId] = word;
            }
        }
        

        // replace placeholders in the template
        string finalBlurb = sample_template.baseText;
        foreach (var kvp in chosenBySlot)
        {
            finalBlurb = finalBlurb.Replace("{" + kvp.Key + "}", kvp.Value);
        }

        Debug.Log(finalBlurb);

    }
    
    string RandomFrom(List<string> list)
    // returns a random item from the provided list
    {
        return list[Random.Range(0, list.Count)];
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
