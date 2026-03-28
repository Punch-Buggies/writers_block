using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Color = UnityEngine.Color;


public class BookCover : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI cornerText;
    // randomize, randomize
    [SerializeField] private Image bg_image; //this is the solid color that gets set from a random setting color
    [SerializeField] private Image outer_border;
    [SerializeField] private Image inner_border;
    [SerializeField] private Image texture; // this is the texture that would go on top, from a random genre


    // title to display in prefab
    private string title;
    // corner header string
    private string cornerHeader;
    private Book book;
    Dictionary<string, Color[]> colorDict;
    
    
    public void Initialize(string title, string cornerHeader, Book bookData)
    {

        titleText.text = title;
        cornerText.text = cornerHeader;
        book = bookData;
        
        MakeColorDic();

        // create book cover image according to its attribute
        SetColors(book); // color according to setting
        SetTexture(book); //texture according to genre
    }

    // add button for opening bookview
    public void OnClick(){
        Debug.Log("youre clicking me");
        // book sound is also played in thiw function call
        BookViewManager.Instance.OpenBookView(book);
    }
    ///////////// colors //////////////
    // the Hex() function is used when intiializing the colorDict to convert them straight to type Color
    private Color Hex(string hexcode)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexcode, out color);
        return color;
    }

    private Color RandomlyPickAColor(string setting)
    {
        Debug.Log($"Picking a color for {setting}");
        //find it in the dictionary, randomly pick one of the colors
        Color[] colors = colorDict[setting];
        Debug.Log($"colors is {colors.Length} long");
        Color randomColor = colors[Random.Range(0, colors.Length)];
        return randomColor;
    }

    private void SetColors(Book book)
    {
        // randomly selects the colors for bg and borders
        string setting = book.setting;
        Debug.Log($"please tell me we got the setting {setting}");
        bg_image.color = RandomlyPickAColor(setting);
        outer_border.color = RandomlyPickAColor(setting);
        inner_border.color = RandomlyPickAColor(setting);
        // set corner color
    }
    ///////////// textures //////////////
    private Sprite[] LoadTextures(string genre)
    {
        string foldername = $"Genre/{genre}_";
        Debug.Log($"trying to get into the folder {foldername}");
        Sprite[] textures = Resources.LoadAll<Sprite>(foldername);
        return textures;
    }
    private Sprite RandomlyPickATexture(string genre)
    {
        Debug.Log($"Picking a texture for {genre}");
        // get the images in the genre folder
        Sprite[] textures = LoadTextures(genre);
        if (textures == null)
        {
            Debug.Log($"{genre} had null textures");
        }
        Sprite randomTexture = textures[Random.Range(0, textures.Length)];
        // get opacity
        return randomTexture;
    }
    float GetOpacity(Sprite texture)
    {
        string name = texture.name;
        Debug.Log($"og '{name}'");
        // only need the last item in parts
        string[] splits = name.Split('_');
        Debug.Log(string.Join(" ,", splits));
        Debug.Log(splits[^2]);
        if (float.TryParse(splits[^2], out float alpha))
        {
            // opacity should be the last item??
            if (0 < alpha && alpha <= 1)
            {
                return alpha;
            }
            Debug.LogWarning($"alpha wasn't between 0 and 1 alpha={alpha}");

        }
        Debug.LogWarning($"couldn't extract opacity from {name}");
        return 1f;
    }

    private void SetImageAndOpacity(Image texture, Sprite randomTexture, float alpha)
    {
        // first set the texture
        texture.sprite = randomTexture;

        // make a new color with the right alpha
        Color c = texture.color;
        // set the alpha
        c.a = alpha;
        // update the color
        texture.color = c;
    }

    private void SetTexture(Book book)
    {
        //randomly selects a texture from its specific genre texture folder
        string genre = book.genre;
        // get a texture
        Sprite randomTexture = RandomlyPickATexture(genre);
        // extract the alpha value
        float alpha = GetOpacity(randomTexture);
        Debug.Log($"for {randomTexture.name}, alpha is {alpha}");
        
        // set texture(image)'s image with the random texture and opacity
        SetImageAndOpacity(texture, randomTexture, alpha);
    }


///////////// color dictionary //////////////
    private void MakeColorDic()
    {
        colorDict = new Dictionary<string, Color[]>
        {
            // uses the helper Hex function to convert hexcode string to a Color object
            { "OuterSpace", new Color[] { Hex("#0B1026"), Hex("#2E0854"), Hex("#00D9FF") } },
            { "Office", new Color[] { Hex("#D1D5DB"), Hex("#F3E5AB"), Hex("#1F2937") } },
            { "Pre-historic", new Color[] { Hex("#5C4033"), Hex("#4B5320"), Hex("#E3DAC9") } },
            { "Mountains", new Color[] { Hex("#708090"), Hex("#F0F8FF"), Hex("#2D5A27") } },
            { "Forest", new Color[] { Hex("#223311"), Hex("#889933"), Hex("#4B3621") } },
            { "Underwater", new Color[] { Hex("#001219"), Hex("#00AFB9"), Hex("#FCFFCB") } },
            { "WildWest", new Color[] { Hex("#C27E3A"), Hex("#4A3728"), Hex("#7B3F40") } },
            { "Medieval", new Color[] { Hex("#7A7D7D"), Hex("#AD9F7C"), Hex("#8B0000") } },
            { "Campus", new Color[] { Hex("#A64439"), Hex("#F9C74F"), Hex("#264653") } },
            { "Hospital", new Color[] { Hex("#E0F2F1"), Hex("#008080"), Hex("#D32F2F") } },
            { "Basement", new Color[] { Hex("#4B4B4B"), Hex("#8B5A2B"), Hex("#FFBF00") } }
        };
    }
}
