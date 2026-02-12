using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set;}

    public int genreStoryElementsUnlocked = 1;
    public int settingsStoryElementsUnlocked = 1;
    public int charactersStoryElementsUnlocked = 1;
    public int tilesUnlocked = 2;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetUnlockedStoryElementCount(string type)
    {
        switch (type)
        {
            case "Genre":
                return genreStoryElementsUnlocked;
            case "Settings":
                return settingsStoryElementsUnlocked;
            case "Character":
                return charactersStoryElementsUnlocked;
            default:
                Debug.LogError("Please make sure, you are calling the function with appropriate type (Genre, Settings, Character)");
                return 0;
        }
    }

    public int GetTilesUnlockedCount()
    {
        return tilesUnlocked;
    }

    public void UpgradeStoryElementCount(string type)
    {
        switch (type)
        {
            case "Genre":
                genreStoryElementsUnlocked++;
                break;
            case "Settings":
                settingsStoryElementsUnlocked++;
                break;
            case "Character":
                charactersStoryElementsUnlocked++;
                break;
            default:
                Debug.LogError("Please make sure, you are calling the function with appropriate type (Genre, Settings, Character)");
                break;
        }
    }

    public void UpgradeTilesUnlockedCount()
    {
        tilesUnlocked++;
    }


}
