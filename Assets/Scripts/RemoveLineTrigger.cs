using System;
using UnityEngine;

public class RemoveLineTrigger : MonoBehaviour
{
    private SwipeDetector _swipeDetector;

    private void Start()
    {
        _swipeDetector = GameObject.FindObjectOfType<SwipeDetector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "LineCollider")
        {
            if (other.gameObject != null && _swipeDetector.swipeDetected)
            {
                other.gameObject.transform.parent.GetComponent<LineProperties>().lineOrigin.numberOfCreatedLines--;

                foreach (var line in other.gameObject.transform.parent.GetComponent<LineProperties>().lineOrigin.GetComponent<BuildingController>().createdLines)
                {
                    if (line.name.ToString() == other.gameObject.transform.parent.name.ToString())
                    {
                        other.gameObject.transform.parent.GetComponent<LineProperties>().lineOrigin.GetComponent<BuildingController>()
                            .createdLines.Remove(line);
                        
                        AudioManager.Instance.RemoveLine();
                        
                        Destroy(line.gameObject);
                    }
                }
            }
        }
    }
}
