using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class BestSeller : MonoBehaviour
{
    StoryElementSupplier storyElementSupplier;

    string[] genreElements;
    string[] characterElements;
    string[] settingElements;

    [SerializeField] TextMeshProUGUI genreBestSellerText;
    [SerializeField] TextMeshProUGUI characterBestSellerText;
    [SerializeField] TextMeshProUGUI settingBestSellerText;

    [SerializeField] TextMeshProUGUI bestSellerTimerText;
    [SerializeField] float bestSellerResetTimer = 60f;

    string[] bestSeller = new string[3];

    Vector3 originalScale;
    Material bsFontMaterial;

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
        // get the og scale and font material for juice purposes later
        originalScale = bestSellerTimerText.transform.localScale;
        bsFontMaterial = bestSellerTimerText.fontMaterial;
        bsFontMaterial.SetFloat("_GlowPower", 0f); // make sure glow is off
    }

    void Update()
    {
        bestSellerResetTimer -= Time.deltaTime;
        bestSellerTimerText.text = Mathf.CeilToInt(bestSellerResetTimer).ToString();
        if (bestSellerResetTimer <= 10f)
        {
            // in the last ten seconds highlight to let them know
            // maybe make it big too
            PulseAndGlow(bestSellerResetTimer);
            
        } 
        if (bestSellerResetTimer <= 0)
        {
            // change elements and reset timer
            SetBestSeller();
            bestSellerResetTimer = 60f;
            StopPulseAndGlow();
        }
    }

    void PulseAndGlow(float time)
    // makes the bestseller timer text pulse and glow for JUICEEEEE
    {
        // get the fraction of current second
        float fraction = time % 1f;
        // scaled to grow to maximum in a second
        float pulse = 1f + 0.25f * Mathf.Sin(Mathf.PI * (1f - fraction));
    
        bestSellerTimerText.transform.localScale = originalScale * pulse;

        bsFontMaterial.SetColor("_GlowColor", Color.red);
        bsFontMaterial.SetFloat("_GlowPower", pulse);
        bsFontMaterial.SetFloat("_GlowOffset", -0.64f);
        bsFontMaterial.SetFloat("_GlowOuter", 1f);
        bsFontMaterial.SetFloat("_GlowInner", 1f);
    }

    void StopPulseAndGlow()
    {
        bestSellerTimerText.transform.localScale = originalScale;
        bsFontMaterial.SetFloat("_GlowPower", 0f); // don't need to change glow color bc when power is 0 you dont see it
    }

    void SetBestSeller()
    {
        // changes the best seller elements
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
        
        return matchCount;
        
    }
}
