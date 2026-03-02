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

    [SerializeField] private int startingAmount = 150;

    public int currentMoney { get; private set; }

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
           Debug.Log("ahhh I'm being destroyed");
           Destroy(gameObject);
           return; 
        }

        Debug.Log("I have $" + currentMoney);
        moneyUI.text = $"${currentMoney}";
    }

    public void addMoney(int amount)
    {
        currentMoney += amount;
        moneyUI.text = $"${currentMoney}";
    }

    public void deductMoney(int amount)
    {
        currentMoney -= amount;
        moneyUI.text = $"${currentMoney}";
    }

    public int getMoney()
    {
        return currentMoney;
    }


}