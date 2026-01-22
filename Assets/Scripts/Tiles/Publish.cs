using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Publish : MonoBehaviour, IDropHandler
{
    GameObject publishedElement;

    [SerializeField] GameObject copiesSoldText;


    float timer = 5f;

    bool startCopiesSoldTimer = false;
    public void OnDrop(PointerEventData eventData)
    {
        publishedElement = eventData.pointerDrag;
        DraggableItem draggable = publishedElement.GetComponent<DraggableItem>();

        if (draggable != null)
        {
            draggable.OnSuccessfulDrop(transform.position);
            copiesSoldText.SetActive(true!);
            startCopiesSoldTimer = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        copiesSoldText.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startCopiesSoldTimer)
        {
            copiesSoldTimer();
        }
        
    }

    void copiesSoldTimer()
    {
        float iniTimer = timer;
        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            timer = iniTimer;
            copiesSoldText.SetActive(false);
            startCopiesSoldTimer = false;
        }
    }
}
