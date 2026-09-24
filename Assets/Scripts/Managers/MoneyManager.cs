using System.Diagnostics.Contracts;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// This script is attached to the MoneyUI gameObject btw
/// </summary>
public class MoneyManager : MonoBehaviour
{
    // only moneymanager script can set moneymanager
    public static MoneyManager Instance { get; private set;}

    public TextMeshProUGUI moneyUI;

    [SerializeField] private double startingAmount = 150;

    public double currentMoney { get; private set; }
    // call MoneyManager.Instance.currentMoney to access this attribute

    double displayMoney;
    int moneyUpdateSpeed = 7;

    public float moneyLimit = 999999;
    public double tileUnlockCost { get; private set; } = 155;

    public double spawnUnlockCost { get; private set; } = 30;

    public double tileGrowthCost {get; private set; } = 50;

    public GameObject passiveIncome;

    void Awake()
    {

        // Debug.Log($"babe moneymanager is awake in {gameObject.scene.name}");

        // if it exists but its not this, destroy
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);

            currentMoney = startingAmount;
        }
        else if (Instance != this)
        {
           // Debug.Log("ahhh I'm being destroyed");
           Destroy(gameObject);
           return; 
        }

        Debug.Log("I have $" + currentMoney);
        displayMoney = currentMoney;
        moneyUI.text = $"${currentMoney:0.##}";
    }


    void Update()
    {
        if(displayMoney != currentMoney)
        {
            displayMoney = Mathf.Lerp((float)displayMoney, (float)currentMoney, Time.deltaTime * moneyUpdateSpeed);
            moneyUI.text = $"${displayMoney:0.##}";
        }
    }

    public void addMoney(double amount)
    {
        if(currentMoney < moneyLimit)
        {
            currentMoney += amount;
            moneyUI.text = $"${currentMoney:0.##}";
            passiveIncome.SetActive(true);
        }
        else
        {
            currentMoney = moneyLimit;
            passiveIncome.SetActive(false);
        }
    }

    public void deductMoney(double amount)
    {
        currentMoney -= amount;
        moneyUI.text = $"${currentMoney:0.##}";
    }

   public void doubleMoney(double amount)
    {
        currentMoney = amount * 2;
        moneyUI.text = $"${currentMoney:0.##}";
    }


    public void increaseTileCost()
    // this is called in Tile.cs
    {
        tileUnlockCost *= 1.5;
    }

    public void increaseSpawnCost()
    // this called in SpawnLocation.cs
    {
        spawnUnlockCost *= 1.5;
    }
}