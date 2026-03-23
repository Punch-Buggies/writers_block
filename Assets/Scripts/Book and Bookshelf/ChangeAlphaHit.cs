///This script is to attach to a button image
///where you want only the opaue part of the
///image to be clickable

using UnityEngine;
using UnityEngine.UI;

public class AlphaHit : MonoBehaviour
{
   [SerializeField] public Image image; 
    void Start()
    {

        image.alphaHitTestMinimumThreshold = 0.1f;
    }


}