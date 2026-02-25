using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] bool spawnOccupied = false;
    [SerializeField] bool unlocked = false;

    [SerializeField] GameObject unlockText;

    int unlockCost = 10;


    void Start()
    {
        if (unlocked)
        {
            UnlockLocation();
        }
        unlockText.SetActive(false);
    }


    public void SetOccupation(bool occupied)
    {
        spawnOccupied = occupied;
    }

    public bool GetOccupation()
    {
        return spawnOccupied; 
    }

    public bool GetUnlockStatus()
    {
        return unlocked;
    }

    public Transform GetTransform()
    {
        return transform;    
    }

    void UnlockLocation()
    {
        Image lockedImage = GetComponent<Image>();
        lockedImage.enabled = false;
        unlockText.SetActive(false);
        unlocked = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        unlockText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unlockText.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(MoneyManager.Instance.getMoney() >= unlockCost)
        {
            if (!unlocked)
            {
                MoneyManager.Instance.deductMoney(unlockCost);
                UnlockLocation();
            }
        }

    }
}
