using System;
using UnityEngine.UI;
using UnityEngine;

public class ClampName : MonoBehaviour
{
    public Transform lifeText;
    public Transform lineIndicators;

    private Transform lastBlock;

    void Update()
    {
        lastBlock = GetComponent<BuildingController>().blocks[GetComponent<BuildingController>().blocks.Count - 1]
            .transform;
        
        if (lastBlock != null)
        {
            Vector3 lifeTextPos = Camera.main.WorldToScreenPoint(lastBlock.position);
            
            lifeText.position = lifeTextPos;
            lineIndicators.position = lifeTextPos;
        }

        lifeText.gameObject.transform.GetChild(0).GetComponent<Text>().text = "" + gameObject.GetComponent<BuildingController>().life;
    }
}
