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
        Dictionary<string, Word> chosenBySlot = new Dictionary<string, Word>();

        //include genre, setting, and character as words
        chosenBySlot["character"] = new Word(character, RandomFrom(new List<Gender>{Gender.masculine,Gender.feminine,Gender.nonbinary}));
        chosenBySlot["genre"] = new Word(genre);
        chosenBySlot["setting"] = new Word(setting);

        // replace independant variables
        List<TemplateSlot> dependants = new List<TemplateSlot>{};
        foreach (var slot in sample_template.slots)
        {
            if (!chosenBySlot.ContainsKey(slot.slotId))//don't overwrite already chosen words
            { 
                bool is_dependant = false;
                string word_s = "s";
                Word word = new Word(word_s);
                switch (slot.type)
                {
                    case WordType.Adjective:
                        word_s = RandomFrom(bookBlurbSupplier.GetAdjectives(character));
                        word = new Word(word_s);
                        break;
                    case WordType.Catchphrase:
                        word_s = RandomFrom(bookBlurbSupplier.GetCatchphrases(character));
                        word = new Word(word_s);
                        break;
                    case WordType.Name:
                    // TODO: restructure name into being a dependant word
                        word_s = RandomFrom(bookBlurbSupplier.GetNames(chosenBySlot["character"].gender));
                        word = new Word(word_s, gender:chosenBySlot["character"].gender);
                        break;
                    case WordType.Person:
                        word_s = RandomFrom(bookBlurbSupplier.GetPeople(setting));
                        word = new Word(word_s, RandomFrom(new List<Gender>{Gender.masculine,Gender.feminine,Gender.nonbinary}));
                        break;
                    case WordType.Place:
                        word_s = RandomFrom(bookBlurbSupplier.GetPlaces(setting));
                        word = new Word(word_s);
                        break;
                    case WordType.Thing:
                        word_s = RandomFrom(bookBlurbSupplier.GetThings(setting));
                        word = new Word(word_s);
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
            string word_s = "s";
            Word word = chosenBySlot[slot.parentId];
            switch (slot.type)
            {
                case WordType.IndefiniteArticle:
                    word = new Word(chosenBySlot[slot.parentId].startsVowel?"an":"a");
                    break;
                case WordType.Pronoun:
                    Gender g = chosenBySlot[slot.parentId].gender;
                    string p = "they";
                    if (g == Gender.feminine)
                    {
                        if (slot.perspective == Perspective.firstPerson){p="she";}
                        if (slot.perspective == Perspective.secondPerson){p="her";}
                        if (slot.perspective == Perspective.thirdPerson){p="hers";}
                    }
                    if (g == Gender.masculine)
                    {
                        if (slot.perspective == Perspective.firstPerson){p="he";}
                        if (slot.perspective == Perspective.secondPerson){p="him";}
                        if (slot.perspective == Perspective.thirdPerson){p="his";}
                    }
                    if (g == Gender.nonbinary)
                    {
                        if (slot.perspective == Perspective.firstPerson){p="they";}
                        if (slot.perspective == Perspective.secondPerson){p="them";}
                        if (slot.perspective == Perspective.thirdPerson){p="theirs";} 
                    }
                    word = new Word(p, g);
                    break;
            }
            chosenBySlot[slot.slotId] = word;
        }
        // replace placeholders in the template
        string finalBlurb = sample_template.baseText;
        foreach (var kvp in chosenBySlot)
        {
            finalBlurb = finalBlurb.Replace("{" + kvp.Key + "}", kvp.Value.word_s);
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

