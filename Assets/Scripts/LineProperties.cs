using System;
using UnityEngine;

public class LineProperties : MonoBehaviour
{
    public enum Type
    {
        Player,
        Opponent
    };

    [Header("Line Type")] 
    [SerializeField] private Type type;
    
    [Header("Prefabs")] 
    [SerializeField] private Transform[] humanPrefabs;
    
    [Header("Line Properties")] 
    public BuildingController lineOrigin;
    public BuildingController lineTarget;
    
    [SerializeField] private float xOffSetSpeed;

    [HideInInspector] public bool activateOffsetMoving;

    private LineRenderer line;

    private bool oneTime;

    [HideInInspector] public float rateTime;

    private float showLineTime;
    
    Transform clone;

    private void Start()
    {
        line = GetComponent<LineRenderer>();

        showLineTime = Time.time + 0.5f;
    }

    private void Update()
    {
        if(activateOffsetMoving)
            SetUpMovingLine();
        if (showLineTime < Time.time)
            gameObject.GetComponent<LineRenderer>().enabled = true;
    }

    void SetUpMovingLine()
    {
        if (!oneTime && type == Type.Player)
        {
            line.material = GameManager.Instance.movingLinePath;
            line.startWidth = 0.04f;
            
            oneTime = true;
        }

        if (rateTime < Time.time)
        {
            CloneHuman();

            rateTime = Time.time + lineOrigin.cloneRateTime;
        }

        float xOffset = -xOffSetSpeed * Time.time;
        
        line.material.mainTextureOffset = new Vector2(xOffset,1);
    }

    void CloneHuman()
    {
        if(lineOrigin.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Simple)
          clone = Instantiate(humanPrefabs[0], lineOrigin.transform.position, Quaternion.identity);
        if(lineOrigin.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Shield)
            clone = Instantiate(humanPrefabs[1], lineOrigin.transform.position, Quaternion.identity);
        if(lineOrigin.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Sword)
            clone = Instantiate(humanPrefabs[2], lineOrigin.transform.position, Quaternion.identity);

        clone.GetComponent<StickmanController>().startPoint = lineOrigin.gameObject.transform;
        clone.GetComponent<StickmanController>().targetPoint = lineTarget.gameObject.transform;

        clone.parent = this.transform;

        Vector3 dir = lineTarget.transform.position - lineOrigin.transform.position;
        
        Quaternion rotation = Quaternion.LookRotation(new Vector3(dir.x,0.0f,dir.z),Vector3.up);
        clone.rotation = rotation;
    }
}
