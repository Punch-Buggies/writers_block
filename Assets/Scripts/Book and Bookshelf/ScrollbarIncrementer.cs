using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollbarIncrementer : MonoBehaviour
{
    //  this script is adapted from thormond's unity discussion post 
    //  https://discussions.unity.com/t/scrolling-scroll-rect-with-buttons/565381/4
    public Scrollbar Target;
    public ScrollRect TargetContent;
    public Button TheOtherButton;
    public float PixelStep = 50f;          // pixels per increment
    public float HoldFrequency = 0.1f;
    [SerializeField] bool increment;
    public void OnPointerDown()
    {
        IncrementDecrementSequence();//invoke the scroll once immediately to avoid delay
        InvokeRepeating("IncrementDecrementSequence", 0.5f, HoldFrequency);
    }

    public void OnPointerUp()
    {
        //stop scrolling
        CancelInvoke("IncrementDecrementSequence");
    }

    public void IncrementDecrementSequence()
    {
        if(increment) Increment();
        else          Decrement();
    }

    public void Increment()
    {
        if (Target == null || TheOtherButton == null) throw new Exception("Setup ScrollbarIncrementer first!");

        GetComponent<Button>().interactable = Target.value != 1;

        //get step size dynamically from size of scroll rect
        RectTransform content = TargetContent.content;
        RectTransform viewport = TargetContent.viewport;

        float contentSize = content.rect.height;
        float viewportSize = viewport.rect.height;
        float scrollableSize = Mathf.Max(1f, contentSize - viewportSize);
        float dynamicStep = PixelStep / scrollableSize;

        //move scrollbar dynamicStep forward
        Target.value = Mathf.Clamp(Target.value + dynamicStep, 0, 1);

        Debug.Log("scroll forward");
        AudioManager.Instance.PlaySFX("increment");
    }

    public void Decrement()
    {
        if (Target == null || TheOtherButton == null) throw new Exception("Setup ScrollbarIncrementer first!");
        
        GetComponent<Button>().interactable = Target.value != 0;
        
        //get step size dynamically from size of scroll rect
        RectTransform content = TargetContent.content;
        RectTransform viewport = TargetContent.viewport;

        float contentSize = content.rect.height;
        float viewportSize = viewport.rect.height;
        float scrollableSize = Mathf.Max(1f, contentSize - viewportSize);
        float dynamicStep = PixelStep / scrollableSize;

        //move scrollbar dynamicStep backward
        Target.value = Mathf.Clamp(Target.value - dynamicStep, 0, 1);

        Debug.Log("scroll backward");
        AudioManager.Instance.PlaySFX("decrement");

    }
     private void MoveContent(float deltaX)
    {
        RectTransform content = TargetContent.content;
        RectTransform viewport = TargetContent.viewport;

        Vector2 pos = content.anchoredPosition;

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;

        float maxScroll = Mathf.Max(0, contentWidth - viewportWidth);

        // Apply movement to content
        pos.x = Mathf.Clamp(pos.x + deltaX, 0, maxScroll);
        content.anchoredPosition = pos;

        // Update button states
        GetComponent<Button>().interactable = (pos.y > 0 && pos.y < maxScroll);

        if (TheOtherButton != null)
            TheOtherButton.interactable = true;
    }
}
