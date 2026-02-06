using UnityEngine;
using System.Collections.Generic;

public class StoryElementSupplier : MonoBehaviour
{
    [SerializeField] private TextAsset storyElementCSV;
    [SerializeField] string[] genreTypes;
    [SerializeField] string[] characterTypes;
    [SerializeField] string[] settingTypes;

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

        // the CSV file should just contain the story type, its value, and associated information like base money amount, and art/audio assests
        string[] lines = storyElementCSV.text.Split('\n'); // Line Separation

        int num_headers = lines[0].Split(',').Length;
        Debug.Log("STORY CSV Number of headers " + num_headers + ", total number of lines " + lines.Length);
    
        // start at i1 because i0 is the headers
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            // Debug.Log($"i{i} {line}");

            if (string.IsNullOrEmpty(line))
            {
                UnityEngine.Debug.LogError("There is an empty line? perhaps at the bottom, please delete it.");
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

            switch (category.ToLower()) // Adding data to its category list
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

}
