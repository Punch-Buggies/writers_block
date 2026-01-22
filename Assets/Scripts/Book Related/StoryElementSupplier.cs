using UnityEngine;

public class StoryElementSupplier : MonoBehaviour
{
    [SerializeField] string[] genreTypes;
    [SerializeField] string[] characterTypes;
    [SerializeField] string[] settingTypes;


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
