using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PassiveIncomeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Changed by into Prefab
    public enum NotificationType
    {
        Publish,        // New book published
        Royalty,        // Passive income from existing book
        Bestseller      // Royalty with bestseller bonus
    }

    // Data structure for cleaner notification handling
    private struct NotificationData
    {
        public string message;
        public NotificationType type;
        public Color color;

        public NotificationData(string msg, NotificationType notifType, Color col)
        {
            message = msg;
            type = notifType;
            color = col;
        }
    }

    //Notification color settings
    [Header("Notification Colors")]
    [SerializeField] private Color publishColor = new Color(1f, 0.84f, 0f);      // Gold for publish
    [SerializeField] private Color royaltyColor = new Color(0.8f, 0.8f, 0.8f);   // Light gray for royalty
    [SerializeField] private Color bestsellerColor = new Color(0.2f, 1f, 0.4f);  // Green for bestseller

    // Animation settings
    [Header("Animation Settings")]
    [SerializeField] private float verticalSpacing = 40f; // Spacing between stacked messages
    [SerializeField] private float moveUpDuration = 0.3f; // Duration for existing messages to move up
    [SerializeField] private float fadeInDuration = 0.3f; // Duration for new message to fade in
    [SerializeField] private float publishPopDuration = 0.25f; // Duration for publish pop animation
    [SerializeField] private float publishPopScale = 1.15f; // How much bigger it gets

    // Published books (source of passive royalties)

    [Header("Prefab Settings")]
    [SerializeField] private GameObject incomeNotificationPrefab; // Prefab Income Notification Text
    [SerializeField] private Transform incomeNotificationsContainer; // Parent container for the text objects
    [SerializeField] private float passiveTimer = 5f; // Time between passive income ticks (seconds)
    [SerializeField] private double passiveIncomeAmount = 20f; // Base passive income per book

    // Internal state
    private Dictionary<string, Book> publishedBooks = new Dictionary<string, Book>();
    private List<TextMeshProUGUI> activeIncomeTexts = new List<TextMeshProUGUI>();
    private int maxVisibleMessages = 3;

    private BestSeller bestSeller;
    void Start()
    {
        bestSeller = FindAnyObjectByType<BestSeller>();

        // Safety checks
        if (incomeNotificationPrefab == null)
        {
            Debug.LogError("PassiveIncomeManager: incomeNotificationPrefab is not assigned.");
        }

        if (incomeNotificationsContainer == null)
        {
            Debug.LogError("PassiveIncomeManager: incomeNotificationsContainer is not assigned.");
        }

        StartCoroutine(IncomePassivelyAfterSomeTime());
    }

    // Public API: Called when a book is published
    public void OnBookPublished(string title, Book book, double initialSaleAmount)
    {
        Debug.Log($"[PassiveIncomeManager] OnBookPublished called! Title: {title}, Amount: ${initialSaleAmount}");

        if (book == null)
        {
            Debug.LogWarning("[PassiveIncomeManager] OnBookPublished: book is null!");
            return;
        }

        if (string.IsNullOrEmpty(title))
        {
            Debug.LogWarning("[PassiveIncomeManager] OnBookPublished: title is null or empty!");
            return;
        }

        // Register the book for passive royalties
        publishedBooks[title] = book;
            Debug.Log($"[PassiveIncomeManager] Book registered. Total published books: {publishedBooks.Count}");

            // Money manager—adjust to formula
            MoneyManager.Instance.addMoney(initialSaleAmount);

        // Emit the initial sale notification (same stacked style)
        string message = $"Published \"{title}\"! +${initialSaleAmount:F0}";
        ShowNotification(message, NotificationType.Publish);
    }



    // Public API for showing passive-income (in case it breaks anything externally)
    public void AddBookToPassiveIncome(string title, Book book)
    {
        if (book == null || string.IsNullOrEmpty(title)) return;
        publishedBooks[title] = book;
    }

    // Unified notification with type support
    public void ShowNotification(string message, NotificationType type)
    {
        Color color = GetColorForType(type);
        NotificationData data = new NotificationData(message, type, color);
        PlayIncomePopup(data);
    }

    // Get appropriate color based on notification type
    private Color GetColorForType(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Publish:
                return publishColor;
            case NotificationType.Bestseller:
                return bestsellerColor;
            case NotificationType.Royalty:
            default:
                return royaltyColor;
        }
    }

    // Passive income coroutine
    IEnumerator IncomePassivelyAfterSomeTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveTimer);

            // Debug.Log("Passive income time!");
            if (publishedBooks.Count > 0)
            {
                // Pick a random book
                List<string> keys = new List<string>(publishedBooks.Keys);
                string randomTitle = keys[Random.Range(0, keys.Count)];
                Book book = publishedBooks[randomTitle];

                // Check how many bestseller this book hits
                string[] bestSellers = bestSeller.GetBestSellers();
                int bestsellerHits = 0;
                foreach (string bs in bestSellers)
                {
                    if (book.genre == bs || book.character == bs || book.setting == bs)
                    {
                        bestsellerHits++;
                    }
                }

                // Compute payout
                double payout = passiveIncomeAmount * (bestsellerHits + 1);
                MoneyManager.Instance.addMoney(payout);

                // Use appropriate notification type based on bestseller hits
                string bestsellerMessage = bestsellerHits > 0
                    ? $" ({bestsellerHits} bestseller hit{(bestsellerHits > 1 ? "s" : "")})"
                    : "";

                string message = $"\"{randomTitle}\" earned +${payout:F0} royalties{bestsellerMessage}";

                // Different type for bestseller vs normal royalty
                NotificationType type = bestsellerHits > 0 ? NotificationType.Bestseller : NotificationType.Royalty;
                ShowNotification(message, type);
            }
        }
    }

    // Creating stacked income notifications per Mike's suggestion 
    void PlayIncomePopup(NotificationData data)
    {
        Debug.Log($"[PassiveIncomeManager] PlayIncomePopup: Type={data.type}, Message='{data.message}'");
        
        if (incomeNotificationPrefab == null || incomeNotificationsContainer == null)
        {
            Debug.LogError("[PassiveIncomeManager] Prefab or Container not assigned!");
            return; // guard clause
        }

        // Remove oldest if at max capacity
        if (activeIncomeTexts.Count >= maxVisibleMessages)
        {
            var oldestText = activeIncomeTexts[0];
            activeIncomeTexts.RemoveAt(0);

            oldestText.DOKill();
            oldestText.DOFade(0f, fadeInDuration).OnComplete(() => Destroy(oldestText.gameObject));
        }

        // Move existing messages up and reduce their opacity
        for (int i = 0; i < activeIncomeTexts.Count; i++)
        {
            var text = activeIncomeTexts[i];
            text.DOKill(); // prevent tween overlap on existing notifications

            // Move up by one position
            float newYPos = -verticalSpacing * i;
            text.rectTransform.DOAnchorPosY(newYPos, moveUpDuration).SetEase(Ease.OutCubic);

            // Opacity levels: newest=100%, middle=80%, oldest=60%
            float targetAlpha = 1f - (0.2f * (activeIncomeTexts.Count - 1 - i));
            text.DOFade(targetAlpha, moveUpDuration);
        }

        // Spawn new message at the bottom position
        var newTextObj = Instantiate(incomeNotificationPrefab, incomeNotificationsContainer);
        var newText = newTextObj.GetComponent<TextMeshProUGUI>();

        if (newText == null) // safety check
        {
            Debug.LogError("PassiveIncomeManager: incomeNotificationPrefab is missing a TextMeshProUGUI component.");
            Destroy(newTextObj);
            return;
        }

        // Position and configure
        // Position at bottom = -verticalSpacing * current count
        float bottomYPos = -verticalSpacing * activeIncomeTexts.Count;
        newText.rectTransform.anchoredPosition = new Vector2(0f, bottomYPos);
        newText.text = data.message;
        newText.color = data.color;  // Apply color based on type
        newText.alpha = 0f;

        // Special pop animation for publish notifications
        if (data.type == NotificationType.Publish)
        {
            newText.rectTransform.localScale = Vector3.one * 0.85f;

            Sequence publishSeq = DOTween.Sequence();
            publishSeq.Append(newText.DOFade(1f, fadeInDuration));
            publishSeq.Join(newText.rectTransform.DOScale(publishPopScale, publishPopDuration * 0.5f).SetEase(Ease.OutBack));
            publishSeq.Append(newText.rectTransform.DOScale(1f, publishPopDuration * 0.5f).SetEase(Ease.InOutQuad));
        }
        else
        {
            // Standard fade-in for royalty notifications
            newText.rectTransform.localScale = Vector3.one;
            newText.DOFade(1f, fadeInDuration);
        }
        // Add to the list
        activeIncomeTexts.Add(newText); 
    }
}


// This is the untouched older script in case the game breaks - 

//// using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using DG.Tweening;

//public class PassiveIncomeManager : MonoBehaviour
//{
//    // Start is called once before the first execution of Update after the MonoBehaviour is created

//    Dictionary<string, Book> publishedBooks;

//    [SerializeField] TextMeshProUGUI passiveIncomeText;
//    [SerializeField] float verticalFloatDistance = 60f;
//    // this change how much the text moves up(postive number) or down(negative number)
//    [SerializeField] float floatDuration;
//    float passiveTimer = 3f;
//    double passiveIncomeAmount = 20f;
//    Vector2 originalTextPos;

//    BestSeller bestSeller;
//    void Start()
//    {
//        publishedBooks = new Dictionary<string, Book>();
//        bestSeller = FindAnyObjectByType<BestSeller>();
//        passiveIncomeText.text = "";
//        passiveIncomeText.alpha = 0f;
//        originalTextPos = passiveIncomeText.rectTransform.anchoredPosition;
//        StartCoroutine(IncomePassivelyAfterSomeTime());
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // commenting this debug, because it makes my laptop explode
//        // foreach(string title in publishedBooks.Keys)
//        // {
//        //     if(publishedBooks.Count > 0)
//        //         Debug.Log(title);
//        // }
//    }

//    public void AddBookToPassiveIncome(string title, Book book)
//    {
//        publishedBooks.Add(title, book);
//    }

//    IEnumerator IncomePassivelyAfterSomeTime()
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(passiveTimer);

//            // Debug.Log("Passive income time!");
//            if (publishedBooks.Count > 0)
//            {
//                // Pick a random book
//                List<string> keys = new List<string>(publishedBooks.Keys);
//                string randomTitle = keys[Random.Range(0, keys.Count)];
//                Book book = publishedBooks[randomTitle];

//                // Check how many bestseller this book hits
//                string[] bestSellers = bestSeller.GetBestSellers();
//                int bestsellerHits = 0;
//                foreach (string bs in bestSellers)
//                {
//                    if (book.genre == bs || book.character == bs || book.setting == bs)
//                    {
//                        bestsellerHits++;
//                    }
//                }
//                MoneyManager.Instance.addMoney(passiveIncomeAmount * (bestsellerHits + 1));


//                string bestsellerMessage = "";
//                if (bestsellerHits > 0)
//                {
//                    bestsellerMessage = $" ({bestsellerHits} bestseller hit{(bestsellerHits > 1 ? "s" : "")})";
//                }
//                PlayIncomePopup($"+${passiveIncomeAmount * (bestsellerHits + 1)} from \"{randomTitle}\"{bestsellerMessage}!");
//            }
//        }
//    }
//    void PlayIncomePopup(string message)
//    {
//        // Kill any existing tweens on the text to avoid overlap
//        passiveIncomeText.DOKill();

//        // Reset position and alpha before animating
//        passiveIncomeText.rectTransform.anchoredPosition = originalTextPos;
//        passiveIncomeText.alpha = 1f;
//        passiveIncomeText.text = message;

//        Sequence seq = DOTween.Sequence();

//        // Float upward by 60 units over 1.2s
//        seq.Append(passiveIncomeText.rectTransform
//            .DOAnchorPosY(originalTextPos.y + verticalFloatDistance, floatDuration)
//            .SetEase(Ease.OutCubic));

//        // Fade out during the last 0.6s of the float
//        seq.Insert(0.6f, passiveIncomeText
//            .DOFade(0f, 0.6f)
//            .SetEase(Ease.InQuad));
//    }
//}
