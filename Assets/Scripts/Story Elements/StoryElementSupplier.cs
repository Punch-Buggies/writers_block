using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class StoryElementSupplier : MonoBehaviour
{
    [SerializeField] private TextAsset storyElementCSV;
    [SerializeField] string[] genreTypes;
    [SerializeField] string[] characterTypes;
    [SerializeField] string[] settingTypes;

    // [SerializeField] private Dictionary<string, string[]> elementBlurbs;


    void Awake()
    {
        LoadDataFromCSV();
    }

    void LoadDataFromCSV()
    {
        if (storyElementCSV == null)
        {
            UnityEngine.Debug.LogError("Assign the CSV file, Goddamn it!");
            return;
        }

        List<string> genres = new List<string>();
        List<string> characters = new List<string>();
        List<string> settings = new List<string>();
        // // elementBlurb is the dictionary for associated words
        // elementBlurbs = new Dictionary<string, string[]>();

        string[] lines = storyElementCSV.text.Split('\n'); // Line Separation

        int num_headers = lines[0].Split('\t').Length;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
            {
                UnityEngine.Debug.LogError("We don't take empty lines here, folks!");
                continue;
            }
            
            string[] columns = line.Split(','); // Column Separation

            if (columns.Length < num_headers)
            {
                UnityEngine.Debug.LogError("Please fill all the data, seems like you are missing something? -_-");
                continue;                
            }

            string category = columns[0].Trim();
            string elementName = columns[1].Trim();
            // string[] associatedWords = columns[2].Split('|'); 
            //UnityEngine.Debug.Log($"associatedWords? [{string.Join(", ", associatedWords)}]");



            switch (category.ToLower()) // Filling in the data
            {
                case "genre":
                    genres.Add(elementName);
                    break;
                case "character":
                    characters.Add(elementName);
                    break;
                case "setting":
                    settings.Add(elementName);
                    break;
            }

            // if (!elementBlurbs.ContainsKey(elementName)) // Filling in the text/blurb
            // {
            //     elementBlurbs.Add(elementName, associatedWords);
            // }

            genreTypes = genres.ToArray();
            characterTypes = characters.ToArray();
            settingTypes = settings.ToArray();
        }

    }


    public string[] GetGenreTypes()
    {
        return genreTypes;
    }
    public string[] GetCharacterTypes()
    {
        return characterTypes;
    }
    public string[] GetSettingTypes()
    {
        return settingTypes;
    }

    // public Dictionary<string, string[]> GetElementBlurbs()
    // {
    //     return elementBlurbs;
    // }
}
