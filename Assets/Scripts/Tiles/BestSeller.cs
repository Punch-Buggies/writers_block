using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BestSeller : MonoBehaviour
{
    StoryElementSupplier storyElementSupplier;

    string[] genreElements;
    string[] characterElements;
    string[] settingElements;


    [SerializeField] TextMeshProUGUI genreBestSellerText;
    [SerializeField] TextMeshProUGUI characterBestSellerText;
    [SerializeField] TextMeshProUGUI settingBestSellerText;


    string[] bestSeller = new string[3];

    void Awake()
    {
        storyElementSupplier = FindAnyObjectByType<StoryElementSupplier>();

        genreElements = storyElementSupplier.GetGenreTypes();
        characterElements = storyElementSupplier.GetCharacterTypes();
        settingElements = storyElementSupplier.GetSettingTypes();

    }
    void Start()
    {
        SetBestSeller();
    }

    void SetBestSeller()
    {
        int randomElementNumber = Random.Range(0, genreElements.Length);
        bestSeller[0] = genreElements[randomElementNumber];
        genreBestSellerText.text = bestSeller[0];

        randomElementNumber = Random.Range(0, characterElements.Length);
        bestSeller[1] = characterElements[randomElementNumber];
        characterBestSellerText.text = bestSeller[1];

        randomElementNumber = Random.Range(0, settingElements.Length);
        bestSeller[2] = settingElements[randomElementNumber];
        settingBestSellerText.text = bestSeller[2];
    }

    public int BestSellerMultiplicationCalc(Dictionary<string, string> publishedBook)
    {
        // First we check how many match there are
        int matchCount = 0;

        if(publishedBook["Genre"] == bestSeller[0])
            matchCount++;

        if(publishedBook["Character"] == bestSeller[1])
            matchCount++;

        if(publishedBook["Setting"] == bestSeller[2])
            matchCount++;

        

        SetBestSeller();
        
        return matchCount;
        
    }
}
