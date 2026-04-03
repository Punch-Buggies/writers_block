using UnityEngine;

public class MouseTrailing : MonoBehaviour
{

    void Start()
    {
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        // Mouse Position
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
        
    }
}
