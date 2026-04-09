using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Diagnostics;

/*
Class for creating Books.
This is data only.
Info can be read and passed to UI objects.
*/

[System.Serializable]
public class Book
{
    public string genre;
    public string character;
    public string setting;


    public int copiesSold;
    public int moneyMade;
    public int bestSelling;
    // [SerializeField] BookBlurbGenerator bookBlurbGenerator;
    public string blurb;

    // initialize
    public Book(string genre, string character, string setting, int copiesSold, int moneyMade, int bestSelling)
    {
        this.genre = genre;
        this.character = character;
        this.setting = setting;
        this.copiesSold = copiesSold;
        this.moneyMade = moneyMade;
        this.bestSelling = bestSelling; // this is a number between 0 and 3 on how many hits it had for a best seller


        // increasing money because we sold something
        UnityEngine.Debug.Log($"Before: ${MoneyManager.Instance.currentMoney}");
        MoneyManager.Instance.addMoney(this.moneyMade);
        UnityEngine.Debug.Log($"Published! ${MoneyManager.Instance.currentMoney}");
    }
}