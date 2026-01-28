using UnityEngine;
using System.Collections.Generic;

public class StoryElementSupplier : MonoBehaviour
{
    [SerializeField] private TextAsset storyElementCSV;
    [SerializeField] string[] genreTypes;
    [SerializeField] string[] characterTypes;
    [SerializeField] string[] settingTypes;

    [SerializeField] private Dictionary<string, string> elementBlurbs;


    void Awake()
    {
        LoadDataFromCSV();
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

    public Dictionary<string, string> GetElementBlurbs()
    {
        return elementBlurbs;
    }

    void LoadDataFromCSV()
    {
        if (storyElementCSV == null)
        {
            Debug.LogError("Assign the CSV file, Goddamn it!");
            return;
        }

        List<string> genres = new List<string>();
        List<string> characters = new List<string>();
        List<string> settings = new List<string>();
        elementBlurbs = new Dictionary<string, string>();

        string[] lines = storyElementCSV.text.Split('\n'); // Line Separation

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
            {
                Debug.LogError("We don't take empty lines here, folks!");
                continue;
            }
            
            string[] columns = line.Split(','); // Column Separation

            if (columns.Length < 3)
            {
                Debug.LogError("Please fill all the data, seems like you are missing something? -_-");
                continue;                
            }

            string category = columns[0].Trim();
            string elementName = columns[1].Trim();
            string description = columns[2].Trim();



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

            if (!elementBlurbs.ContainsKey(elementName)) // Filling in the text/blurb
            {
                elementBlurbs.Add(elementName, description);
            }

            genreTypes = genres.ToArray();
            characterTypes = characters.ToArray();
            settingTypes = settings.ToArray();
        }

    }
}
