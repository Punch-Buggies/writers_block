using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SpawnLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] bool spawnOccupied = false;
    [SerializeField] bool unlocked = false;

    [SerializeField] GameObject unlockText;

    [SerializeField] int unlockCost = 10;
    [SerializeField]TextMeshProUGUI unlockCostText;


    void Start()
    {
        if (unlocked)
        {
            UnlockLocation();
        }
        unlockText.SetActive(false);
        unlockCostText.text = "";

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
        unlockCostText.text = "$ " + unlockCost.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unlockText.SetActive(false);
        unlockCostText.text = "";
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
