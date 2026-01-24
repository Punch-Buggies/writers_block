using UnityEngine;
using System.Collections.Generic;

public class BookBlurbSupplier : MonoBehaviour
// handles associated word dictionaries used to generate blurbs 
// dictionary structure:
//      setting_words = {key:storyelement = [[people], [places], [things]]}
//      character_words = {key:storyelement = [[adjectives], [catchphrases]]}
//      names = {key:gender = [names]}
{
    Dictionary<string, string[][]> setting_words;
    Dictionary<string, string[][]> character_words;
    Dictionary<string, string[]> names;

    void Awake()
    // initialize dictionaries
    {
        setting_words = new Dictionary<string, string[][]>();
        character_words = new Dictionary<string, string[][]>();
        names = new Dictionary<string, string[]>();

        BuildSampleData();
    }

    void BuildSampleData()
    // sample data for testing purposes, actual data will link to csv
    {
        setting_words.Add("Spaceship", new string[][]
        {
            new string[] { "alien", "captain", "stormtrooper" },
            new string[] { "control room", "research lab", "med bay" },
            new string[] { "lightsaber", "spacesuit", "beaker" }
        });

        setting_words.Add("Castle", new string[][]
        {
            new string[] { "king", "jester", "knight", "squire" },
            new string[] { "throne room", "stable", "royal courtyard", "ballroom" },
            new string[] { "crown", "jewel", "broadsword", "flag" }
        });

        character_words.Add("Villain", new string[][]
        {
            new string[] { "evil", "somber", "piercing", "twisted" },
            new string[] { "I will avenge my dead father!", "Pity. You're too late!" }
        });

        character_words.Add("Business_Person", new string[][]
        {
            new string[] { "plain", "greedy", "bored", "soulless" },
            new string[] { "Let me crunch the numbers...", "Have my secretary schedule that in for next week." }
        });

        names.Add("f", new string[] { "Mary", "Lottie", "Amelia", "Pauline", "Molly", "Harriet", "Leah", "Astrid" });
        names.Add("m", new string[] { "Bob", "Reggie", "Reginald", "Barty", "John", "Maverick", "Nicholas", "Xavier" });
        names.Add("nb", new string[] { "Alex", "Loren", "Avery", "Stardust", "Steel Lightning" });
    }
}
