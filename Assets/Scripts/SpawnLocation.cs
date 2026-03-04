using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SpawnLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] bool spawnOccupied = false;
    [SerializeField] bool unlocked = false;

    [SerializeField] GameObject unlockText;

    double unlockCost;
    [SerializeField]TextMeshProUGUI unlockCostText;


    void Start()
    {
        if (unlocked)
        {
            UnlockLocation();
        }
        unlockText.SetActive(false);
        unlockCostText.text = "";

        unlockCost = MoneyManager.Instance.spawnUnlockCost;
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
        // get unlock cost
        unlockCost = MoneyManager.Instance.spawnUnlockCost; 

        unlockText.SetActive(true);
        unlockCostText.text = $"${unlockCost:0.##}";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unlockText.SetActive(false);
        unlockCostText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
         // get unlock cost
        unlockCost = MoneyManager.Instance.spawnUnlockCost;

        if(MoneyManager.Instance.currentMoney >= unlockCost && unlocked == false)
        {
        
            MoneyManager.Instance.deductMoney(unlockCost);
            UnlockLocation();
            //update cost
            MoneyManager.Instance.increaseSpawnCost();
    
        }

    }
}
