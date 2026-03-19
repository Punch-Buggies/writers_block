using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollbarIncrementer : MonoBehaviour
{
    //  this script is adapted from thormond's unity discussion post 
    //  https://discussions.unity.com/t/scrolling-scroll-rect-with-buttons/565381/4
    public Scrollbar Target;
    public Button TheOtherButton;
    public float Step = 0.1f;
    public float HoldFrequency = 0.1f;
    [SerializeField] bool increment;
    public void OnPointerDown()
    {
        InvokeRepeating("IncrementDecrementSequence", 0.5f, HoldFrequency);
    }

    public void OnPointerUp()
    {
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
        Target.value = Mathf.Clamp(Target.value + Step, 0, 1);
        GetComponent<Button>().interactable = Target.value != 1;
        TheOtherButton.interactable = true;
        Debug.Log("scroll forward");
    }

    public void Decrement()
    {
        if (Target == null || TheOtherButton == null) throw new Exception("Setup ScrollbarIncrementer first!");
        Target.value = Mathf.Clamp(Target.value - Step, 0, 1);
        GetComponent<Button>().interactable = Target.value != 0;
        TheOtherButton.interactable = true;
        Debug.Log("scroll backward");
    }
}
