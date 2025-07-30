using System;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    public enum State
    {
        Free,
        Busy
    }

    public State buildingState;
    
    [SerializeField] private LayerMask _mask;
    [SerializeField] private LineRenderer linePrefab;
    
    public bool isDragging;

    private GameObject highlightSprite;
    
    private Transform targetSprite;

    private bool oneTime;
    void Start()
    {
        isDragging = false;

        highlightSprite = transform.GetChild(0).gameObject;
        targetSprite = transform.GetChild(1).transform;
    }
    void Update()
    {
        if (isDragging && gameObject.GetComponent<BuildingController>().type == BuildingController.Type.Player && GameManager.Instance.gameState == GameManager.GameState.Gameplay)
        {
            if (!oneTime)
            {
                GameManager.Instance.CreateLine();
                oneTime = true;
            }
            if(GameManager.Instance.currentPathLine != null)
                DrawLinePath();

            if (Input.GetMouseButtonUp(0) && GameManager.Instance.currentSelectedBuilding.name.ToString() ==
                this.gameObject.name.ToString())
            {
                isDragging = false;
                oneTime = false;

                GameManager.Instance.isDragging = false;

                if (buildingState == State.Busy && GameManager.Instance.insideBuild &&
                    GameManager.Instance.currentPathLine != null)
                    DestroyLine();

                if (!GameManager.Instance.insideBuild || GameManager.Instance.insideBuild && GameManager.Instance
                        .currentSelectedBuilding.GetComponent<BuildingController>().numberOfCreatedLines
                    >= GameManager.Instance.currentSelectedBuilding.GetComponent<BuildingController>()
                        .numberOfLinesSupported)
                {
                    if (GameManager.Instance.currentPathLine != null)
                        DestroyLine();
                }

                if (GameManager.Instance.insideBuild && GameManager.Instance.currentSelectedBuilding
                        .GetComponent<BuildingController>().numberOfCreatedLines
                    < GameManager.Instance.currentSelectedBuilding.GetComponent<BuildingController>()
                        .numberOfLinesSupported && GameManager.Instance.currentPathLine != null)
                {
                    if (GameManager.Instance.currentPathLine != null)
                    {
                        GameManager.Instance.currentPathLine.gameObject.GetComponent<LineProperties>().lineOrigin =
                            GameManager.Instance.currentSelectedBuilding.GetComponent<BuildingController>();
                        GameManager.Instance.currentPathLine.gameObject.GetComponent<LineProperties>().lineTarget =
                            GameManager.Instance.targetSelectedBuilding.GetComponent<BuildingController>();
                    }

                    if (GameManager.Instance.currentPathLine != null)
                        AddColliderToLine(GameManager.Instance.currentPathLine, highlightSprite.transform.position,
                            targetSprite.position);

                    GameManager.Instance.currentSelectedBuilding.GetComponent<BuildingController>()
                        .numberOfCreatedLines++;

                    GameManager.Instance.currentPathLine.SetPosition(1, new Vector3(GameManager.Instance.targetSelectedBuilding
                        .transform.GetChild(0).transform.position.x,-1.4081f,GameManager.Instance.targetSelectedBuilding
                        .transform.GetChild(0).transform.position.z));

                    GameManager.Instance.targetSelectedBuilding.gameObject.GetComponent<BuildingController>()
                        .stopLifeIncreasing = true;

                    GameManager.Instance.currentPathLine.name =
                        GameManager.Instance.currentSelectedBuilding.name.ToString() +
                        GameManager.Instance.targetSelectedBuilding.ToString();

                    if (!GameManager.Instance.CheckIfLineExist(GameManager.Instance.currentPathLine.name.ToString()))
                    {
                        GameManager.Instance.avaiableLines.Add(GameManager.Instance.currentPathLine.gameObject);
                        gameObject.GetComponent<BuildingController>().createdLines.Add(GameManager.Instance.currentPathLine.gameObject);
                    }
                    else
                        Destroy(GameManager.Instance.currentPathLine.gameObject);

                    GameManager.Instance.currentPathLine.GetComponent<LineProperties>().activateOffsetMoving = true;
                }

                highlightSprite.SetActive(false);
                targetSprite.gameObject.SetActive(false);
            }
        }
    }

    private void OnMouseDown()
    {
        if (gameObject.GetComponent<BuildingController>().type == BuildingController.Type.Player)
        {
            GameManager.Instance.currentSelectedBuilding = null;
            GameManager.Instance.insideBuild = false;
        }
    }

    private void OnMouseDrag()
    {
        if (gameObject.GetComponent<BuildingController>().type == BuildingController.Type.Player)
        {
            isDragging = true;

            GameManager.Instance.isDragging = true;
            GameManager.Instance.currentSelectedBuilding = this.gameObject;

            highlightSprite.SetActive(true);
            targetSprite.gameObject.SetActive(true);
        }
    }

    void DrawLinePath()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitPoint;

        if (Physics.Raycast(ray, out hitPoint,100f, _mask))
        {
            targetSprite.position = new Vector3(hitPoint.point.x,-1.405f,hitPoint.point.z);
        }

        if (GameManager.Instance.currentSelectedBuilding.GetComponent<BuildingController>().numberOfCreatedLines
            >= GameManager.Instance.currentSelectedBuilding.GetComponent<BuildingController>().numberOfLinesSupported)
            GameManager.Instance.currentPathLine.material = GameManager.Instance.redMaterial;
        else
            GameManager.Instance.currentPathLine.material = GameManager.Instance.blueMaterial;

        GameManager.Instance.currentPathLine.SetPosition(0,new Vector3(highlightSprite.transform.position.x,-1.4079f,highlightSprite.transform.position.z));
        GameManager.Instance.currentPathLine.SetPosition(1, new Vector3(targetSprite.position.x, -1.408f, targetSprite.position.z));
    }
    

    void DestroyLine()
    {
        Destroy(GameManager.Instance.currentPathLine.gameObject);

        targetSprite.position = highlightSprite.transform.position;
        
        AudioManager.Instance.WrongBuilding();
    }
    private void AddColliderToLine(LineRenderer line, Vector3 startPoint, Vector3 endPoint)
    {
        BoxCollider lineCollider = new GameObject("LineCollider").AddComponent<BoxCollider>();
        lineCollider.transform.parent = line.transform;

        lineCollider.gameObject.AddComponent<LineCollider>();

        float lineWidth = line.endWidth; 

        float lineLength = Vector3.Distance(startPoint, endPoint);      

        lineCollider.size = new Vector3(lineLength, lineWidth, 0.1f);   

        Vector3 midPoint = (startPoint + endPoint) / 2;

        lineCollider.transform.position = midPoint;

        float angle = Mathf.Atan2((endPoint.z - startPoint.z), (endPoint.x - startPoint.x));

        angle *= Mathf.Rad2Deg;

        angle *= -1; 

        lineCollider.transform.Rotate(0, angle, 0);
        
        AudioManager.Instance.RightBuilding();
    }
}
