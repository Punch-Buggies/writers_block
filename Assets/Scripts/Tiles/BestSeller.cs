using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BestSeller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
    [SerializeField] TextMeshProUGUI bestSellerText; // this it the title text
    [SerializeField] Publish publish;
    [SerializeField] GameObject genreHighlight;
    [SerializeField] GameObject characterHighlight;
    [SerializeField] GameObject settingsHighlight;
    [SerializeField] GameObject darkBG;


    public string[] bestSeller = new string[3];

    Vector3 originalScale;
    Material bsFontMaterial;
    Material titleFontMaterial;
    // this was for highlight polish but i didnt end up using it
    Material charMaterial;
    Material settMaterial;
    Material genMaterial;
    Color32 matchColor = new Color32(191, 191, 0, 255);


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
        titleFontMaterial = bestSellerText.fontMaterial;

        genMaterial = genreBestSellerText.fontMaterial;
        charMaterial = characterBestSellerText.fontMaterial;
        settMaterial = settingBestSellerText.fontMaterial;


        // make sure glow is off
        bsFontMaterial.SetFloat("_GlowPower", 0f); 
        titleFontMaterial.SetFloat("_GlowPower", 0f); 
        genMaterial.SetFloat("_GlowPower", 0f);
        charMaterial.SetFloat("_GlowPower", 0f);
        settMaterial.SetFloat("_GlowPower", 0f);

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
            // check for highlights bc they might still be valid
            publish.CheckBSMatch();
        }
    }

    void PulseAndGlow(float time)
    // makes the bestseller timer text pulse and glow for JUICEEEEE
    {
        // get the fraction of current second
        float fraction = time % 1f;
        // scaled to grow to maximum in a second
        float pulse = 1f + 0.25f * Mathf.Sin(Mathf.PI * (1f - fraction));

        // make timer glow and pulse
        bestSellerTimerText.transform.localScale = originalScale * pulse;
        bsFontMaterial.SetColor("_GlowColor", Color.red);
        bsFontMaterial.SetFloat("_GlowPower", pulse);
        bsFontMaterial.SetFloat("_GlowOffset", -0.64f);
        bsFontMaterial.SetFloat("_GlowOuter", 1f);
        bsFontMaterial.SetFloat("_GlowInner", 1f);

        // make text glow to the pulse but not actually pulse
        titleFontMaterial.SetColor("_GlowColor", Color.red);
        titleFontMaterial.SetFloat("_GlowPower", pulse);
        titleFontMaterial.SetFloat("_GlowOffset", -0.64f);
        titleFontMaterial.SetFloat("_GlowOuter", 1f);
        titleFontMaterial.SetFloat("_GlowInner", 1f);
    }

    void StopPulseAndGlow()
    {
        bestSellerTimerText.transform.localScale = originalScale;
        bsFontMaterial.SetFloat("_GlowPower", 0f); // don't need to change glow color bc when power is 0 you dont see it
        titleFontMaterial.SetFloat("_GlowPower", 0f);

    }

    public void MaterialMatchGlow(string storyElement, bool glow)
    {
        // assign material accordingly
        // Material material = null;
        // Debug.Log("Setting the material to change");
        GameObject material = null;
        switch (storyElement)
        {
            case "Genre":
                // material = genMaterial;
                material = genreHighlight;
                break;
            case "Character":
                // material = charMaterial;
                material = characterHighlight;
                break;
            case "Setting":
                // material = settMaterial;
                material = settingsHighlight;
                break;
            default:
                Debug.Log("somethingwent weird in amterial match glow");
                break;
        }
        if (material != null && glow == true)
        {
            Debug.Log("makign it glow baby");
            material.GetComponent<Image>().enabled = true;
            // set the specific material to glow
            // material.SetColor("_GlowColor", matchColor);
            // material.SetFloat("_GlowPower", 0.47f);
            // material.SetFloat("_GlowOffset", 0.14f);
            // material.SetFloat("_GlowOuter", 0.15f);
        }
        else if (material != null && glow == false)
        {
            // material.SetFloat("_GlowPower", 0f);
            material.GetComponent<Image>().enabled = false;
        }
        else
        {
            Debug.Log("material is null");
        }
    }

    public string[]  GetBestSellers()
    {
        string genreBS = genreBestSellerText.text;
        string charBS = characterBestSellerText.text;
        string settBS = settingBestSellerText.text;

        return new string[] {genreBS, charBS, settBS};
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
    public void OnPointerClick(PointerEventData eventData)
    {
        // when this is clicked, check toggleINFO on
        if (PurchaseManager.Instance.toggleOnInfo == true)
        {
            AudioManager.Instance.PlaySFX("click");
            // display info
            string text = $"Book profit multiplies for each tile in the publish zone that matches its bestseller. Resets every minute.";
            PurchaseManager.Instance.DisplayTileInfo(text);
            }
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // if mouse enters area && info is on, turn on dark bg
        if (PurchaseManager.Instance.toggleOnInfo == true)
        {
            darkBG.SetActive(true);
        }

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // if pointer leaves area, turn off dark bg
        darkBG.SetActive(false);
    }
}
