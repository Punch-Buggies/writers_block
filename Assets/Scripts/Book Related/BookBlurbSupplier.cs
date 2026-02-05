using UnityEngine;
using System.Collections.Generic;

public class BookBlurbSupplier : MonoBehaviour
// handles associated word dictionaries and templates used to generate blurbs 
// dictionary structure:
//   setting_words   = { key: settingType   -> SettingWordSet }
//   character_words = { key: characterType -> CharacterWordSet }
//   names           = { key: gender        -> [names] }
//   templates       = { key: genre         -> [BookBlurbTemplate] }
{
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

        BuildSampleData(); // todo: replace with CSV retrieval
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
        templates.Add("Test", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate(
                "{pronoun} saw {a} {person1}",
                new List<TemplateSlot>
                {   
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("pronoun", WordType.Pronoun, parentId:"character", perspective:Perspective.firstPerson),
                    new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"person1")
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
                    new TemplateSlot("name1", WordType.Name),
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
                    new TemplateSlot("name1", WordType.Name),
                    new TemplateSlot("his", WordType.Pronoun, parentId:"character", perspective:Perspective.thirdPerson),
                    new TemplateSlot("he", WordType.Pronoun, parentId:"character", perspective:Perspective.firstPerson),
                    new TemplateSlot("him", WordType.Pronoun, parentId:"character", perspective:Perspective.secondPerson),
                    new TemplateSlot("his_posses", WordType.Pronoun, parentId:"character", perspective:Perspective.possessive)
                },
                "Romance"
            ),
            new BookBlurbTemplate(
                "{name1} surged forward, catching {name2}'s lips in {a1} {adjective1} kiss. The {person1}'s lips parted formed a surprised 'o'. {name1} slid {their} tongue forward, watching {name2}'s eyes curiously.",
                new List<TemplateSlot>
                {
                    new TemplateSlot("name1", WordType.Name),
                    new TemplateSlot("name2", WordType.Name),
                    new TemplateSlot("adjective1", WordType.Adjective),
                    new TemplateSlot("person1", WordType.Person),
                    new TemplateSlot("a1", WordType.IndefiniteArticle, parentId:"adjective1"),
                    new TemplateSlot("their", WordType.Pronoun, parentId:"character", perspective:Perspective.possessive)
            
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