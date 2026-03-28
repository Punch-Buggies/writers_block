using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Text;

public class BookBlurbGenerator : MonoBehaviour
{

    [SerializeField] BookBlurbSupplier bookBlurbSupplier;//gives associated words
    public static BookBlurbGenerator Instance { get; private set;}

    private static readonly HashSet<string> SmallWords = new HashSet<string>
    {
        "a", "an", "the", "and", "but", "or", "in", "for", "of"
    };


    public string generate_blurb(string genre, string character, string setting)
    {   
        HashSet<string> usedWords = new HashSet<string>(); //track words that have been used by their id

        // choose template
        var possible_templates = bookBlurbSupplier.GetTemplates(genre);
        BookBlurbTemplate sample_template = possible_templates[Random.Range(0, possible_templates.Count)];

        // map slotId -> chosen word
        Dictionary<string, Word> chosenBySlot = new Dictionary<string, Word>();

        //include genre, setting, and character as words
        chosenBySlot["character"] = new Word(character, RandomFrom(new List<Gender>{Gender.masculine,Gender.feminine, Gender.nonbinary}));
        usedWords.Add(character);
        chosenBySlot["genre"] = new Word(genre);
        usedWords.Add(genre);
        chosenBySlot["setting"] = new Word(setting);
        usedWords.Add(setting);

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
                        word_s = RandomUniqueFrom(bookBlurbSupplier.GetAdjectives(character), s=>s, usedWords);
                        word = new Word(word_s);
                        break;
                    case WordType.Catchphrase:
                        word_s = RandomUniqueFrom(bookBlurbSupplier.GetCatchphrases(character), s=>s, usedWords);
                        word = new Word(word_s);
                        break;
                    case WordType.Name:
                        is_dependant = true;
                        break;
                    case WordType.Person:
                        word_s = RandomUniqueFrom(bookBlurbSupplier.GetPeople(setting), s=>s, usedWords);
                        if (slot.plural){word_s = NounPluralizer.Pluralize(word_s);}
                        word = new Word(word_s, RandomFrom(new List<Gender>{Gender.masculine,Gender.feminine}));
                        break;
                    case WordType.Place:
                        word_s = RandomUniqueFrom(bookBlurbSupplier.GetPlaces(setting), s=>s, usedWords);
                        if (slot.plural){word_s = NounPluralizer.Pluralize(word_s);}
                        word = new Word(word_s);
                        break;
                    case WordType.Thing:
                        word_s = RandomUniqueFrom(bookBlurbSupplier.GetThings(setting), s=>s, usedWords);
                        if (slot.plural){word_s = NounPluralizer.Pluralize(word_s);}
                        word = new Word(word_s);
                        break;
                    case WordType.Pronoun:
                        is_dependant = true;
                        break;
                    case WordType.IndefiniteArticle:
                        is_dependant = true;
                        break;
                    case WordType.Verb:
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
                throw new System.Exception($"'{slot.slotId}''s Parent slot '{slot.parentId}' not resolved.");
            }
            string word_s = "s";
            Word word = chosenBySlot[slot.parentId];
            Gender g = chosenBySlot[slot.parentId].gender;
            switch (slot.type)
            {
                case WordType.IndefiniteArticle:
                    word = new Word(chosenBySlot[slot.parentId].startsVowel?"an":"a");
                    break;
                case WordType.Verb:
                    string w = "";
                    if (slot.slotId == "is")
                    {
                        w = (g == Gender.nonbinary)?"are":"is";
                    }else if (slot.slotId == "has")
                    {
                        w = (g==Gender.nonbinary)?"have":"has";
                        
                    }else if (slot.slotId == "does")
                    {
                        w = (g==Gender.nonbinary)?"do":"does";
                    }else if (slot.slotId == "was")
                    {
                        w = (g==Gender.nonbinary)?"were":"was";
                    }
                    else
                    {
                        Debug.Log("Conjugated verb slot has undefined or incorrectly defined id: " + slot.slotId);
                    }
                    word = new Word(w,g);
                    break;
                case WordType.Pronoun:
                    string p = "they";
                    if (g == Gender.feminine)
                    {
                        if (slot.perspective == Conjugation.Subject){p="she";}
                        if (slot.perspective == Conjugation.Object){p="her";}
                        if (slot.perspective == Conjugation.PossessivePro){p="hers";}
                        if (slot.perspective == Conjugation.PossessiveAdj){p="her";}
                    }
                    if (g == Gender.masculine)
                    {
                        if (slot.perspective == Conjugation.Subject){p="he";}
                        if (slot.perspective == Conjugation.Object){p="him";}
                        if (slot.perspective == Conjugation.PossessivePro){p="his";}
                        if (slot.perspective == Conjugation.PossessiveAdj){p="his";}
                    }
                    if (g == Gender.nonbinary)
                    {
                        if (slot.perspective == Conjugation.Subject){p="they";}
                        if (slot.perspective == Conjugation.Object){p="them";}
                        if (slot.perspective == Conjugation.PossessivePro){p="theirs";} 
                        if (slot.perspective == Conjugation.PossessiveAdj){p="their";}
                    }
                    word = new Word(p, g);
                    break;
                case WordType.Name:
                        word_s = RandomUniqueFrom(bookBlurbSupplier.GetNames(chosenBySlot[slot.parentId].gender), s=>s, usedWords);
                        word = new Word(word_s, gender:chosenBySlot[slot.parentId].gender);
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
        

        // Convert the string to title case
        if (genre == "Title")
        {
            finalBlurb = CapitalizeTitle(finalBlurb);
        }
        else
        {
            finalBlurb = CapitalizeSentence(finalBlurb);
        }
        
        Debug.Log("Blurb:\n"+finalBlurb);// OUTPUT
        return finalBlurb;
    }
    
    public static string CapitalizeSentence(string input)
    {
        //tldr this function tracks when it finds a punctuation and flips the capitlizeNext bool so on the next iteration it capitalizes whatever follows the punctuation

        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }
            
        StringBuilder result = new StringBuilder(input.Length); //stringBuilder allows for mutable strings
        bool capitalizeNext = true; // start by capitalizing the first character

        foreach (char c in input)
        {
            //capitalize
            if (capitalizeNext && char.IsLetter(c))
            {
                result.Append(char.ToUpper(c));
                capitalizeNext = false;
            }
            //don't capitalize
            else
            {
                result.Append(c);
            }
            // If the character is a punctuation, set bool to capitalize next letter
            if (c == '.' || c == '!' || c == '?')
            {
                capitalizeNext = true;
            }
            // handle quotes: if a punctuation is followed by a quote, capitalize after it
            else if (c == '"' || c == '“' || c == '”')
            {
                // skip
            }
            // Skip spaces and line breaks when capitalizing next letter
            else if (!char.IsWhiteSpace(c) && c != '\r' && c != '\n')
            {
                capitalizeNext = false;
            }
            
        }
        return result.ToString();
    }

    public static string CapitalizeTitle(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        string[] tokens = input.Split(' ');
        StringBuilder result = new StringBuilder();
        bool firstWord = true;

        foreach (string token in tokens)
        {
            string word = token;

            int start = 0;
            while (start < word.Length && !char.IsLetterOrDigit(word[start]) && word[start] != '{')
                start++;

            int end = word.Length - 1;
            while (end >= start && !char.IsLetterOrDigit(word[end]) && word[end] != '}')
                end--;

            if (start <= end)
            {
                string prefix = word.Substring(0, start);
                string core = word.Substring(start, end - start + 1);
                string suffix = word.Substring(end + 1);

                string lowerCore = core.ToLower();

                if (!(core.StartsWith("{") && core.EndsWith("}")))
                {
                    if (firstWord || !SmallWords.Contains(lowerCore))
                    {
                        lowerCore = char.ToUpper(lowerCore[0]) + lowerCore.Substring(1);
                    }
                }

                result.Append(prefix + lowerCore + suffix);
            }
            else
            {
                result.Append(word);
            }

            result.Append(' ');
            firstWord = false;
        }

        return result.ToString().TrimEnd();
    }
    T RandomFrom<T>(List<T> list)
    {
        // returns a random item from the provided list
        return list[Random.Range(0, list.Count)];
    }
    T RandomUniqueFrom<T>(List<T> list,System.Func<T, string> key,HashSet<string> used)
    {
        //returns a unique (not yet in the hash set) random item from the provided list
        if (list == null || list.Count == 0)
        {   
            throw new System.Exception("RandomUniqueFrom called with empty list.");
        }
            
            

        var shuffled = new List<T>(list);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (var item in shuffled)
        {
            string k = key(item);
            if (!used.Contains(k))
            {
                used.Add(k);
                return item;
            }
        }

        throw new System.Exception("No unused words available.");
    }

    void Awake()
    {
        Debug.Log("the generator has awakened");
        Instance = this;
    }
    void Start()
    {
        // TESTING

        string g = "History";
        string c = "Bodyguard";
        string s = "Office";
        for (int i=0; i<10;i++){
          string test = generate_blurb(g,c,s);
        Debug.Log(test);  
        };
        
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

public static class NounPluralizer
//pluralizes any given noun -- update private HashSets if any associated words have irregular pluralisation
{
    private static readonly Dictionary<string, string> Irregular = new()
    {
        {"man","men"},
        {"woman","women"},
        {"child","children"},
        {"person","people"},
        {"mouse","mice"},
        {"goose","geese"},
        {"tooth","teeth"},
        {"foot","feet"},
        {"ox","oxen"},
        {"die","dice"}
    };

    private static readonly HashSet<string> NoChange = new()
    {
        "sheep","deer","fish","aircraft","species","series","bison","paperwork"
    };

    private static readonly HashSet<string> FExceptions = new()
    {
        "roof","belief","chef","chief","proof","reef"
    };

    private static readonly HashSet<string> OExceptions = new()
    {
        "photo","piano","halo","memo","stereo"
    };

    public static string Pluralize(string noun)
    {
        if (string.IsNullOrWhiteSpace(noun))
            return noun;

        bool capitalized = char.IsUpper(noun[0]);
        string word = noun.ToLower();

        // irregular nouns
        if (Irregular.ContainsKey(word))
            return MatchCase(Irregular[word], capitalized);

        // same singular/plural
        if (NoChange.Contains(word))
            return noun;

        // latin/greek patterns
        if (word.EndsWith("is"))
            return MatchCase(word[..^2] + "es", capitalized);   // analysis → analyses

        if (word.EndsWith("us"))
            return MatchCase(word[..^2] + "i", capitalized);    // cactus → cacti

        if (word.EndsWith("um"))
            return MatchCase(word[..^2] + "a", capitalized);    // bacterium → bacteria

        if (word.EndsWith("on"))
            return MatchCase(word[..^2] + "a", capitalized);    // phenomenon → phenomena

        // consonant + y → ies
        if (word.EndsWith("y") && word.Length > 1 && !"aeiou".Contains(word[^2]))
            return MatchCase(word[..^1] + "ies", capitalized);

        // f / fe → ves (with exceptions)
        if (word.EndsWith("fe"))
            return MatchCase(word[..^2] + "ves", capitalized);

        if (word.EndsWith("f") && !FExceptions.Contains(word))
            return MatchCase(word[..^1] + "ves", capitalized);

        // sibilant endings → es
        if (word.EndsWith("s") || word.EndsWith("x") || word.EndsWith("z") ||
            word.EndsWith("ch") || word.EndsWith("sh"))
            return MatchCase(word + "es", capitalized);

        // words ending in o
        if (word.EndsWith("o") && !OExceptions.Contains(word))
            return MatchCase(word + "es", capitalized);

        // default
        return MatchCase(word + "s", capitalized);
    }

    private static string MatchCase(string word, bool capitalized)
    {
        if (!capitalized) return word;
        return char.ToUpper(word[0]) + word.Substring(1);
    }
}