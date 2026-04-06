using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Eraser : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color highlightColor = new Color32(204, 177, 167, 183);
    Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // if mouse enters area && info is on, make it a bit darker
        // ORRRR if they havent made teir first book
        if (PurchaseManager.Instance.toggleOnInfo == true || BookshelfManager.Instance.getBookCount() == 0)
        {
            image.color = highlightColor;
        }

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // if pointer leaves area, turn off dark bg
        image.color = Color.white;
    } 
}
