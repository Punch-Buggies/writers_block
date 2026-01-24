using UnityEngine;
using System.Collections.Generic;

public class BookBlurbSupplier : MonoBehaviour
// handles associated word dictionaries used to generate blurbs 
// dictionary structure:
//      setting_words = {key:storyelement = [[people], [places], [things]]}
//      character_words = {key:storyelement = [[adjectives], [catchphrases]]}
//      names = {key:gender = [names]}
{
    Dictionary<string, List<List<string>>> setting_words;
    Dictionary<string, List<List<string>>> character_words;
    Dictionary<string, List<string>> names;

    void Awake()
    // initialize dictionaries
    {
        setting_words = new Dictionary<string, List<List<string>>>();
        character_words = new Dictionary<string, List<List<string>>>();
        names = new Dictionary<string, List<string>>();

        BuildSampleData();
    }

    public List<string> GetAdjectives(string key)   => character_words[key][0];
    public List<string> GetCatchphrases(string key) => character_words[key][1];

    public List<string> GetPeople(string key) => setting_words[key][0];
    public List<string> GetPlaces(string key) => setting_words[key][1];
    public List<string> GetThings(string key) => setting_words[key][2];

    public List<string> GetNames(string key) => names[key];

    void BuildSampleData()
    // sample data for testing purposes, actual data will link to csv
    {
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
