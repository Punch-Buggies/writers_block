using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PassiveIncomeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Dictionary<string, Book> publishedBooks;

    [SerializeField] TextMeshProUGUI passiveIncomeText;
    [SerializeField] float verticalFloatDistance = 60f; 
    // this change how much the text moves up(postive number) or down(negative number)
    [SerializeField] float floatDuration;
    float passiveTimer = 3f;
    double passiveIncomeAmount = 20f;
    Vector2 originalTextPos;

    BestSeller bestSeller;
    void Start()
    {
        publishedBooks = new Dictionary<string, Book>();
        bestSeller = FindAnyObjectByType<BestSeller>();
        passiveIncomeText.text = "";
        passiveIncomeText.alpha = 0f;

        originalTextPos = passiveIncomeText.rectTransform.anchoredPosition;
        StartCoroutine(IncomePassivelyAfterSomeTime());
    }

    // Update is called once per frame
    void Update()
    {
        // commenting this debug, because it makes my laptop explode
        // foreach(string title in publishedBooks.Keys)
        // {
        //     if(publishedBooks.Count > 0)
        //         Debug.Log(title);
        // }
    }

    public void AddBookToPassiveIncome(string title, Book book)
    {
        publishedBooks.Add(title, book);
    }

    IEnumerator IncomePassivelyAfterSomeTime()
    {
        while(true)
        {
            yield return new WaitForSeconds(passiveTimer);

            // Debug.Log("Passive income time!");
            /* 
            if we have more than one published book,
            randomly pick a 


            */
            if(publishedBooks.Count > 0)
            {
                // Pick a random book
                List<string> keys = new List<string>(publishedBooks.Keys);
                string randomTitle = keys[Random.Range(0, keys.Count)];
                Book book = publishedBooks[randomTitle];

                int bestsellerHits = book.bestSelling;

                // // Check how many bestseller this book hits
                // string[] bestSellers = bestSeller.GetBestSellers();


                // int bestsellerHits = 0;
                // foreach(string bs in bestSellers)
                // {
                //     if(book.genre== bs || book.character == bs || book.setting == bs)
                //     {
                //         bestsellerHits++;
                //     }
                // }


                MoneyManager.Instance.addMoney(passiveIncomeAmount * (bestsellerHits + 1));
                

                string bestsellerMessage = "";
                if (bestsellerHits > 0)
                {
                    bestsellerMessage = $" ({bestsellerHits} bestseller hit{(bestsellerHits > 1 ? "s" : "")})";
                }
                PlayIncomePopup($"+${passiveIncomeAmount * (bestsellerHits + 1)} from \"{randomTitle}\"{bestsellerMessage}!");
            }
        }
    }
    void PlayIncomePopup(string message)
    {
        passiveIncomeText.DOKill();

        // Fade out current text first if visible, then show new one
        if (passiveIncomeText.alpha > 0f)
        {
            Sequence fadeSwap = DOTween.Sequence();
            fadeSwap.Append(passiveIncomeText.DOFade(0f, 0.3f).SetEase(Ease.InQuad));
            fadeSwap.AppendCallback(() =>
            {
                passiveIncomeText.rectTransform.anchoredPosition = originalTextPos;
                passiveIncomeText.text = message;
            });
            fadeSwap.Append(passiveIncomeText.DOFade(1f, 0.3f).SetEase(Ease.OutQuad));
            // Float upward and stay
            fadeSwap.Join(passiveIncomeText.rectTransform
                .DOAnchorPosY(originalTextPos.y + verticalFloatDistance, floatDuration)
                .SetEase(Ease.OutCubic));
        }
        else
        {
            // First time, just fade in and float up
            passiveIncomeText.rectTransform.anchoredPosition = originalTextPos;
            passiveIncomeText.text = message;

            Sequence seq = DOTween.Sequence();
            seq.Append(passiveIncomeText.DOFade(1f, 0.3f).SetEase(Ease.OutQuad));
            seq.Join(passiveIncomeText.rectTransform
                .DOAnchorPosY(originalTextPos.y + verticalFloatDistance, floatDuration)
                .SetEase(Ease.OutCubic));
        }
    }
}
