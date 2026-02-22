using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class BookBlurbSupplier : MonoBehaviour
// handles associated word dictionaries and templates used to generate blurbs 
// dictionary structure:
//   setting_words   = { key: settingType   -> SettingWordSet }
//   character_words = { key: characterType -> CharacterWordSet }
//   names           = { key: gender        -> [names] }
//   templates       = { key: genre         -> [BookBlurbTemplate] }
{
    [SerializeField] private TextAsset blurbCSV;
    Dictionary<string, SettingWordSet> setting_words;
    Dictionary<string, CharacterWordSet> character_words;
    Dictionary<Gender, List<string>> names;
    Dictionary<string, List<BookBlurbTemplate>> templates;

    void Awake()
    {
        setting_words = new Dictionary<string, SettingWordSet>();
        character_words = new Dictionary<string, CharacterWordSet>();
        names = new Dictionary<Gender, List<string>>();
        templates = new Dictionary<string, List<BookBlurbTemplate>>();

        // BuildSampleData(); // todo: replace with CSV retrieval
        BuildDataFromCSV();
    }

    // getters
    public List<string> GetAdjectives(string key)   => character_words[key].adjectives;
    public List<string> GetCatchphrases(string key) => character_words[key].catchphrases;

    public List<string> GetPeople(string key) => setting_words[key].people;
    public List<string> GetPlaces(string key) => setting_words[key].places;
    public List<string> GetThings(string key) => setting_words[key].things;

    public List<string> GetNames(Gender key) => names[key];

    public List<BookBlurbTemplate> GetTemplates(string key) => templates[key];


    void BuildSampleData()
    // sample data for testing
    {
        templates.Add("Title", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate(
                "{character}'s {adjective} {genre} story",
                new List<TemplateSlot>
                {
                    new TemplateSlot("adjective", WordType.Adjective)
                },
                "Title"
            )
        });
        templates.Add("Test", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate(
                "The {first_name} saw {their1} {thing_a}. Then, the {name2}, {they2} took it from {them1}",
                new List<TemplateSlot>
                {   
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("their1", WordType.Pronoun, parentId:"person1", conjugation:Conjugation.PossessiveAdj),
                    new TemplateSlot("thing_a", WordType.Thing),
                    new TemplateSlot("person2", WordType.Person),
                    new TemplateSlot("they2", WordType.Pronoun, parentId:"person2", conjugation:Conjugation.Subject),
                    new TemplateSlot("them1", WordType.Pronoun, parentId:"person1", conjugation:Conjugation.Object),
                    new TemplateSlot("first_name", WordType.Name, parentId:"person1"),
                    new TemplateSlot("name2", WordType.Name, parentId:"person2")
                },
                "Test"
            )
            
        });
        templates.Add("Romance", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate(
                "{name1} kissed the {adjective1} {person1} in the {place1}. But, the {person1} was actually {a} {person2}!",
                new List<TemplateSlot>
                {
                    new TemplateSlot("name1", WordType.Name, parentId:"character"),
                    new TemplateSlot("adjective1", WordType.Adjective),
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("person2", WordType.Person),
                    new TemplateSlot("place1", WordType.Place),
                    new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"person2")
                },
                "Romance"
            ),
            new BookBlurbTemplate(
                "Holding the {thing1} between {his_posses} teeth in a {adjective1} bite, paying careful attention to the {thing2}, {name1}’s hands were now free to wander down {his_posses} lap where {he} quickly got to work unzipping {him}self.",
                new List<TemplateSlot>
                {
                    new TemplateSlot("thing1", WordType.Thing),
                    new TemplateSlot("adjective1", WordType.Adjective),
                    new TemplateSlot("thing2", WordType.Thing),
                    new TemplateSlot("name1", WordType.Name, parentId:"character"),
                    new TemplateSlot("his", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessivePro),
                    new TemplateSlot("he", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
                    new TemplateSlot("him", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
                    new TemplateSlot("his_posses", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj)
                },
                "Romance"
            ),
            new BookBlurbTemplate(
                "{name1} surged forward, catching {name2}'s lips in {a1} {adjective1} kiss. The {person1}'s lips parted formed a surprised 'o'. {name1} slid {their} tongue forward, watching {name2}'s eyes curiously.",
                new List<TemplateSlot>
                {
                    new TemplateSlot("name1", WordType.Name, parentId:"character"),
                    new TemplateSlot("name2", WordType.Name, parentId:"person1"),
                    new TemplateSlot("adjective1", WordType.Adjective),
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("a1", WordType.IndefiniteArticle, parentId:"adjective1"),
                    new TemplateSlot("their", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj)
            
                },
                "Romance"
            ),
        });

        setting_words.Add("Spaceship", new SettingWordSet
        {
            people = new List<string> { "alien", "captain", "stormtrooper" },
            places = new List<string> { "control room", "research lab", "med bay" },
            things = new List<string> { "lightsaber", "spacesuit", "beaker" }
        });

        setting_words.Add("Castle", new SettingWordSet
        {
            people = new List<string> { "king", "jester", "knight", "squire", "imbecile", "ingrate" },
            places = new List<string> { "throne room", "stable", "royal courtyard", "ballroom" },
            things = new List<string> { "crown", "jewel", "broadsword", "flag" }
        });

        character_words.Add("Villain", new CharacterWordSet
        {
            adjectives = new List<string> { "evil", "somber", "piercing", "twisted" },
            catchphrases = new List<string>
            {
                "I will avenge my dead father!",
                "Pity. You're too late!"
            }
        });

        character_words.Add("Business_Person", new CharacterWordSet
        {
            adjectives = new List<string> { "plain", "greedy", "bored", "soulless" },
            catchphrases = new List<string>
            {
                "Let me crunch the numbers...",
                "Have my secretary schedule that in for next week."
            }
        });

        names.Add(Gender.feminine, new List<string> { "Mary", "Lottie", "Amelia", "Pauline", "Molly", "Harriet", "Leah", "Astrid" });
        names.Add(Gender.masculine, new List<string> { "Bob", "Reggie", "Reginald", "Barty", "John", "Maverick", "Nicholas", "Xavier" });
        names.Add(Gender.nonbinary, new List<string> { "Alex", "Loren", "Avery", "Stardust", "Steel Lightning" });
    }

    void addCharacterData(string value,string adjectives, string catchphrases)
    {
        //(1) parse into lists
        List<string> adj = new List<string>(adjectives.Split('|'));
        List<string> cp = new List<string>(catchphrases.Split('|'));
        //(2) add data to dictionary
        character_words.Add(value, new CharacterWordSet{
            adjectives = adj,
            catchphrases = cp    
        });
        // Debug.Log($"{value}\nadjs: {string.Join(", ",GetAdjectives(value))} \ncps: {string.Join(", ", GetCatchphrases(value))}\n");
    }
    void addSettingData(string value, string people, string places, string things)
    {
        //(1) parse into lists
        List<string> peop = new List<string>(people.Split('|'));
        List<string> pla = new List<string>(places.Split('|'));
        List<string> thi = new List<string>(things.Split('|'));
        //(2) add data to dictionary
        setting_words.Add(value, new SettingWordSet
        {
           people = peop,
           places = pla,
           things = thi 
        });
        // Debug.Log($"{value}\npeople: {string.Join(", ",GetPeople(value))} \nplaces: {string.Join(", ", GetPlaces(value))}\nthings: {string.Join(", ", GetThings(value))}\n");
        
    }
    void addTemplateData(string value, string templates)
    {
        //(1) parse into lists
        List<string> temp = new List<string>();
        foreach (Match match in Regex.Matches(templates, "\".*?\""))
        {// regex match for quotations
            temp.Add(match.Value);
        }

    }

    void BuildDataFromCSV()
    {
        /* This function parses through the CSV and adds the info to the corresponding dictionary. First goes through some error checking, then goes through each line and switches to the correct addData function. */
        if (blurbCSV == null)
            {
                UnityEngine.Debug.LogError("You gotta attach the CSV file to the supplier object");
                return;
            }

        /* CSV file is structured as the following: 
        Value, Story Type, Adjectives, Catchphrases, People, Places, Things, Template */
        string[] lines = blurbCSV.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries); // Line Separation
        int num_headers = lines[0].Split(',').Length;
        Debug.Log("BLURB CSV Number of headers " + num_headers + ", total number of entries " + lines.Length);
        Debug.Log($"Last line raw: '{lines[lines.Length - 1]}'");
        
        // starts at i1 bc i0 are headers
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            // Debug.Log($"i{i} {line}");
            if (string.IsNullOrEmpty(line))
            { // check if the line is empty
                UnityEngine.Debug.LogError("There is an empty line? perhaps at the bottom, please delete it.");
                continue;
            }
            string[] columns = line.Split(','); // Column Separation
            if (columns.Length < num_headers)
            { // check for missing column
                UnityEngine.Debug.LogError("Please fill all the data, seems like you are missing something? -_-");
                continue;                
            }

            string value = columns[0].Trim();
            string storyType = columns[1].Trim();

            switch (storyType)
            // grab relevant columns and call addfunction
            {
                case "Setting":
                string people = columns[4].Trim();
                string places = columns[5].Trim();
                string things = columns[6].Trim();
                // all the adding is done here
                addSettingData(value, people, places, things);
                break;
                case "Character":
                string adjectives = columns[2].Trim();
                string catchphrases = columns[3].Trim();
                // all the adding is done here
                addCharacterData(value, adjectives, catchphrases);
                break;
                case "Genre":
                string templates = columns[7].Trim();
                // all the adding is done here
                addTemplateData(value, templates);
                break;
                default:
                Debug.Log("No CSV case found");
                break;
            }
        }
        // adding templates for testing, remove this once the addTemplateData has been made
        templates.Add("Title", new List<BookBlurbTemplate>
        // note title's don't have access to original genre
        {
            new BookBlurbTemplate(
                "{character}'s {adjective} Story",
                new List<TemplateSlot>
                {
                    new TemplateSlot("adjective", WordType.Adjective)
                },
                "Title"
            )
        });
        templates.Add("Test", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate("{adj} {name1} ran over to {name2}, the {person2}. {they} shouted at {them}, “{catchphrase}!", 
            new List<TemplateSlot>
            {
                new TemplateSlot("adj", WordType.Adjective),
                new TemplateSlot("name1", WordType.Name, parentId:"character"),
                new TemplateSlot("name2", WordType.Name, parentId:"person2"),
                new TemplateSlot("person2", WordType.Person),
                new TemplateSlot("them", WordType.Pronoun, parentId:"person2", conjugation:Conjugation.Object),
                new TemplateSlot("catchphrase", WordType.Catchphrase),
                new TemplateSlot("they", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject)
            }, 
            "Test"),
            new BookBlurbTemplate(
                "{first_name}, the {person1}, saw {their1} {thing_a}. Then, {name2} took it from {them1}",
                new List<TemplateSlot>
                {   
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("their1", WordType.Pronoun, parentId:"person1", conjugation:Conjugation.PossessiveAdj),
                    new TemplateSlot("thing_a", WordType.Thing),
                    new TemplateSlot("person2", WordType.Person),
                    new TemplateSlot("they2", WordType.Pronoun, parentId:"person2", conjugation:Conjugation.Subject),
                    new TemplateSlot("them1", WordType.Pronoun, parentId:"person1", conjugation:Conjugation.Object),
                    new TemplateSlot("first_name", WordType.Name, parentId:"person1"),
                    new TemplateSlot("name2", WordType.Name, parentId:"person2")
                },
                "Test"
            )
            
        });
        templates.Add("Romance", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate(
                "{name1} kissed the {adjective1} {person1} in the {place1}. But, the {person1} was actually {a} {person2}!",
                new List<TemplateSlot>
                {
                    new TemplateSlot("name1", WordType.Name, parentId:"character"),
                    new TemplateSlot("adjective1", WordType.Adjective),
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("person2", WordType.Person),
                    new TemplateSlot("place1", WordType.Place),
                    new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"person2")
                },
                "Romance"
            ),
            new BookBlurbTemplate(
                "Holding the {thing1} between {his_posses} teeth in a {adjective1} bite, paying careful attention to the {thing2}, {name1}’s hands were now free to wander down {his_posses} lap where {he} quickly got to work unzipping {him}self.",
                new List<TemplateSlot>
                {
                    new TemplateSlot("thing1", WordType.Thing),
                    new TemplateSlot("adjective1", WordType.Adjective),
                    new TemplateSlot("thing2", WordType.Thing),
                    new TemplateSlot("name1", WordType.Name, parentId:"character"),
                    new TemplateSlot("his", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessivePro),
                    new TemplateSlot("he", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
                    new TemplateSlot("him", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
                    new TemplateSlot("his_posses", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj)
                },
                "Romance"
            ),
            new BookBlurbTemplate(
                "{name1} surged forward, catching {name2}'s lips in {a1} {adjective1} kiss. The {person1}'s lips parted formed a surprised 'o'. {name1} slid {their} tongue forward, watching {name2}'s eyes curiously.",
                new List<TemplateSlot>
                {
                    new TemplateSlot("name1", WordType.Name, parentId:"character"),
                    new TemplateSlot("name2", WordType.Name, parentId:"person1"),
                    new TemplateSlot("adjective1", WordType.Adjective),
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("a1", WordType.IndefiniteArticle, parentId:"adjective1"),
                    new TemplateSlot("their", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj)
            
                },
                "Romance"
            ),
        });
        names.Add(Gender.feminine, new List<string> { "Mary", "Lottie", "Amelia", "Pauline", "Molly", "Harriet", "Leah", "Astrid" });
        names.Add(Gender.masculine, new List<string> { "Bob", "Reggie", "Reginald", "Barty", "John", "Maverick", "Nicholas", "Xavier" });
        names.Add(Gender.nonbinary, new List<string> { "Alex", "Loren", "Avery", "Stardust", "Steel Lightning" });
        Debug.Log("I have populated all the data i Hope");
    }
// end of BookBLurnSupplier Class
}



[System.Serializable]
public class SettingWordSet
{
    public List<string> people = new();
    public List<string> places = new();
    public List<string> things = new();
}

[System.Serializable]
public class CharacterWordSet
{
    public List<string> adjectives = new();
    public List<string> catchphrases = new();
}
public enum Gender
{
    feminine,
    masculine,
    nonbinary
}