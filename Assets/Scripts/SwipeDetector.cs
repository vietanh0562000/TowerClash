using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    public bool swipeDetected;
    public float swipeDeadZone;
    
    private Vector2 startTouch,swipeDelta;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            startTouch = (Vector2)Input.mousePosition;
        if (Input.GetMouseButton(0))
            swipeDelta = (Vector2)Input.mousePosition - startTouch;

        if (swipeDelta.magnitude > swipeDeadZone)
            swipeDetected = true;
        else
            swipeDetected = false;
        
        if(Input.GetMouseButtonUp(0))
            Reset();
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;

        swipeDetected = false;
    }
}
