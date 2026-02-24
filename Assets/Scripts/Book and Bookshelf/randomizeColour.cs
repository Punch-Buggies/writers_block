using UnityEngine;
using UnityEngine.UI;

public class randomizeColour : MonoBehaviour
{
   [SerializeField] public Image imageUi; 
    void Start()
    {
        imageUi.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f);
    }


}
