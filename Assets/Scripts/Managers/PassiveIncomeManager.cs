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
    [SerializeField] float floatDuration = 1.2f;
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
        foreach(string title in publishedBooks.Keys)
        {
            if(publishedBooks.Count > 0)
                Debug.Log(title);
        }
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

            Debug.Log("Passive income time!");
            if(publishedBooks.Count > 0)
            {
                // Pick a random book
                List<string> keys = new List<string>(publishedBooks.Keys);
                string randomTitle = keys[Random.Range(0, keys.Count)];
                Book book = publishedBooks[randomTitle];

                // Check how many bestseller this book hits
                string[] bestSellers = bestSeller.GetBestSellers();
                int bestsellerHits = 0;
                foreach(string bs in bestSellers)
                {
                    if(book.genre== bs || book.character == bs || book.setting == bs)
                    {
                        bestsellerHits++;
                    }
                }
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
        // Kill any existing tweens on the text to avoid overlap
        passiveIncomeText.DOKill();

        // Reset position and alpha before animating
        passiveIncomeText.rectTransform.anchoredPosition = originalTextPos;
        passiveIncomeText.alpha = 1f;
        passiveIncomeText.text = message;

        Sequence seq = DOTween.Sequence();

        // Float upward by 60 units over 1.2s
        seq.Append(passiveIncomeText.rectTransform
            .DOAnchorPosY(originalTextPos.y + verticalFloatDistance, floatDuration)
            .SetEase(Ease.OutCubic));

        // Fade out during the last 0.6s of the float
        seq.Insert(0.6f, passiveIncomeText
            .DOFade(0f, 0.6f)
            .SetEase(Ease.InQuad));
    }
}
