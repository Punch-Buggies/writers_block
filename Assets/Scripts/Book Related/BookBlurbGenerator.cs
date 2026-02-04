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

        //include genre, setting, and character as words
        chosenBySlot["character"] = character;
        chosenBySlot["genre"] = genre;
        chosenBySlot["setting"] = setting;

        // replace independant variables
        List<TemplateSlot> dependants = new List<TemplateSlot>{};
        foreach (var slot in sample_template.slots)
        {
            if (!chosenBySlot.ContainsKey(slot.slotId))//don't overwrite already chosen words
            { 
                bool is_dependant = false;
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
                        word = RandomFrom(bookBlurbSupplier.GetNames(RandomFrom(new List<Gender>{Gender.masculine,Gender.feminine,Gender.nonbinary})));
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
                    case WordType.Pronoun:
                        is_dependant = true;
                        break;
                    case WordType.IndefiniteArticle:
                        is_dependant = true;
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(slot.type), slot.type, null);
                }
                if (is_dependant)
                {
                    dependants.Add(slot);
                }
                else
                {
                   chosenBySlot[slot.slotId] = word; 
                }
                
            }
        }
        
        //handle dependants
        foreach (var slot in dependants)
        {
            if (!chosenBySlot.ContainsKey(slot.parentId))
            {
                throw new System.Exception($"Parent slot '{slot.parentId}' not resolved.");
            }
                
            chosenBySlot[slot.slotId] = chosenBySlot[slot.parentId];
        }
        // replace placeholders in the template
        string finalBlurb = sample_template.baseText;
        foreach (var kvp in chosenBySlot)
        {
            finalBlurb = finalBlurb.Replace("{" + kvp.Key + "}", kvp.Value);
        }

        Debug.Log(finalBlurb);

    }
    
    T RandomFrom<T>(List<T> list)
    {
        // returns a random item from the provided list
        return list[Random.Range(0, list.Count)];
    }


    void Start()
    {
        for (int i = 0; i<5; i++)
        {
            generate_sample();
        }
    }

}


public class Word
{
    public string word_s;
    public Gender gender;
    public bool startsVowel;

    public Word(string word_s, Gender gender = Gender.nonbinary)
    {
        this.word_s = word_s;
        this.gender = gender;
        this.startsVowel = check_starts_vowel(word_s);
    }

    bool check_starts_vowel(string s)
    // returns true if the first letter of a given string is a vowel
    {
        return "aeiouAEIOU".IndexOf(s[0]) >= 0;
    }
}

