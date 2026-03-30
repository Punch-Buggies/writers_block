using UnityEngine.EventSystems;
using UnityEngine;


// this is literally just to highlight the publish button lmfao
public class PublishHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Publish publish;
    [SerializeField] GameObject darkBG;
    
    void Awake()
    {
        publish = FindAnyObjectByType<Publish>();
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
    public void OnPointerClick(PointerEventData eventData)
    {
        // when this is clicked, check toggleINFO on
        if (publish.publishCounter < 3 && PurchaseManager.Instance.toggleOnInfo == true)
        {
            AudioManager.Instance.PlaySFX("click");
            // display info on publish button
            string text = "This is the publish button, when all publish zones have a tile, this button will write your story. Find all your published stories in the bookshelf.";
            PurchaseManager.Instance.DisplayTileInfo(text);
        }
    }
}