using UnityEngine;
using System.Collections.Generic;


public class BookBlurbSupplier : MonoBehaviour
// handles associated word dictionaries and templates used to generate blurbs 
// dictionary structure:
//      setting_words = {key:storyelement = [[people], [places], [things]]}
//      character_words = {key:storyelement = [[adjectives], [catchphrases]]}
//      names = {key:gender = [names]}
//      templates = {key:genre = [Template objects (see BookBlurbTemplate)]}
{
    Dictionary<string, List<List<string>>> setting_words;
    Dictionary<string, List<List<string>>> character_words;
    Dictionary<string, List<string>> names;
    Dictionary<string, List<BookBlurbTemplate>> templates;

    void Awake()
    // initialize dictionaries
    {
        setting_words = new Dictionary<string, List<List<string>>>();
        character_words = new Dictionary<string, List<List<string>>>();
        names = new Dictionary<string, List<string>>();

        BuildSampleData();//replace with csv retrieval
    }

    // getters
    public List<string> GetAdjectives(string key)   => character_words[key][0];
    public List<string> GetCatchphrases(string key) => character_words[key][1];

    public List<string> GetPeople(string key) => setting_words[key][0];
    public List<string> GetPlaces(string key) => setting_words[key][1];
    public List<string> GetThings(string key) => setting_words[key][2];

    public List<string> GetNames(string key) => names[key];

    public List<string> GetTemplates(string key) => templates[key];

    void BuildSampleData()
    // sample data for testing purposes, actual data will link to csv
    {   
        templates.Add("Romance",new BookBlurbTemplate(
        "{name1} kissed the {adjective1} {person1} in the {place1}. But, the {person1} was actually a {person2}!",
        new List<TemplateSlot>
        {
            new TemplateSlot("name1", WordType.Name),
            new TemplateSlot("adjective1", WordType.Adjective),
            new TemplateSlot("person1", WordType.Person),
            new TemplateSlot("person2", WordType.Person),
            new TemplateSlot("place1", WordType.Place),
        }, "Romance"));


        setting_words.Add("Spaceship", new List<List<string>>
        {
            new List<string> { "alien", "captain", "stormtrooper" },
            new List<string> { "control room", "research lab", "med bay" },
            new List<string> { "lightsaber", "spacesuit", "beaker" }
        });

        setting_words.Add("Castle", new List<List<string>>
        {
            new List<string> { "king", "jester", "knight", "squire" },
            new List<string> { "throne room", "stable", "royal courtyard", "ballroom" },
            new List<string> { "crown", "jewel", "broadsword", "flag" }
        });

        character_words.Add("Villain", new List<List<string>>
        {
            new List<string> { "evil", "somber", "piercing", "twisted" },
            new List<string> { "I will avenge my dead father!", "Pity. You're too late!" }
        });

        character_words.Add("Business_Person", new List<List<string>>
        {
            new List<string> { "plain", "greedy", "bored", "soulless" },
            new List<string> { "Let me crunch the numbers...", "Have my secretary schedule that in for next week." }
        });

        names.Add("f", new List<string> { "Mary", "Lottie", "Amelia", "Pauline", "Molly", "Harriet", "Leah", "Astrid" });
        names.Add("m", new List<string> { "Bob", "Reggie", "Reginald", "Barty", "John", "Maverick", "Nicholas", "Xavier" });
        names.Add("nb", new List<string> { "Alex", "Loren", "Avery", "Stardust", "Steel Lightning" });
    }
}
