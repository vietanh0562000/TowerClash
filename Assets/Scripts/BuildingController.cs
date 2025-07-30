using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public enum Type
    {
        Player,
        Opponent,
        Opponent2,
        Neutral
    };

    public enum BuildType
    {
        Simple,
        Shield,
        Sword
    };

    public enum AiOpponentDifficulty
    {
        Easy,
        Medium,
        Hard
    };

    [Header("List of blocks")] 
    public List<Transform> blocks;

    [Header("Linde indicators UI")] 
    [SerializeField] private List<GameObject> indicators;

    [Header("Type of building")] 
    public Type type;
    public BuildType buildType;
    
    [Header("Building Properties")] 
    public int numberOfLinesSupported;
    public float cloneRateTime;
    public int life;
    public int numberOfBlocks;
    public float dBetweenBlocks;
    public Color buildingColor;
    public List<GameObject> createdLines;

    [Header("Materials")] 
    public Material baseMaterial;
    public Material topBlockMaterial;
    public Material baseMaterialShield;
    public Material topBlockMaterialShield;
    public Material baseMaterialSword;
    public Material topBlockMaterialSword;

    [Header("Prefabs")] 
    [SerializeField] private Transform block;
    public int numberOfCreatedLines;

    [Header("Ai Building Components")] 
    public List<Transform> freeBuildings;
    [SerializeField] private Transform[] linePrefab;
    [SerializeField] private bool createNewLine;
    [SerializeField] private List<Transform> busyBuildings;

    [Header("Ai Building Properties")] 
    [SerializeField] private AiOpponentDifficulty buildingDificulty;
        
    private Transform _highlightSprite;

    private float rateTime;
    private float aiTime;

    private bool aiOneTime;
    public bool aiStopCreateLines;
    
    Transform lineClone;

    [HideInInspector] public bool stopLifeIncreasing;

    private void Awake()
    {
        SetUpCanvasScaler();
    }

    void Start()
    {
        _highlightSprite = transform.GetChild(0).transform;
        
        ChangeMaterials(this.gameObject.GetComponent<BuildingController>(),this.gameObject.GetComponent<BuildingController>());
        InitBuildingsWithBlocks();
    }

    private void Update()
    {
        SetUpLineIdicatorsUI();

        if (GameManager.Instance.gameState == GameManager.GameState.Gameplay)
        {
            if (life <= 65)
            {
                numberOfBlocks = life / 6 + 1;

                if (numberOfBlocks > blocks.Count)
                    CloneBuildingBlock(new Vector3(blocks[blocks.Count - 1].position.x,
                        blocks[blocks.Count - 1].position.y + dBetweenBlocks, blocks[blocks.Count - 1].position.z));
                if (numberOfBlocks < blocks.Count)
                    RemoveBuildingBlock();

                if (life > 15 && life < 40)
                {
                    numberOfLinesSupported = 2;
                    cloneRateTime = 1.0f;
                }

                if (life >= 40)
                {
                    numberOfLinesSupported = 3;
                    cloneRateTime = 0.5f;
                }

                if (life <= 15)
                {
                    numberOfLinesSupported = 1;
                    cloneRateTime = 2.0f;

                }

                if (numberOfCreatedLines <= 0 && rateTime < Time.time && type != Type.Neutral)
                {
                    rateTime = Time.time + cloneRateTime + 1.0f;
                    life++;

                    GameManager.Instance.totalPoints++;

                    if (type == Type.Player)
                        GameManager.Instance.playerPoints++;
                }
            }
            else
            {
                life = 65;
            }

            //Activate Draw Path script if it is player building
            if (type == Type.Player)
                gameObject.GetComponent<DrawPath>().enabled = true;
            else
                gameObject.GetComponent<DrawPath>().enabled = false;

            #region AI Building

            if (type == Type.Opponent || type == Type.Opponent2)
            {
                if (buildingDificulty == AiOpponentDifficulty.Easy && !aiOneTime)
                {
                    aiTime = Time.time + 10f;
                    aiOneTime = true;

                    createNewLine = true;

                    if (numberOfCreatedLines == 1)
                        aiStopCreateLines = true;
                    else
                        aiStopCreateLines = false;
                }

                if (buildingDificulty == AiOpponentDifficulty.Medium && !aiOneTime)
                {
                    aiTime = Time.time + 7f;
                    aiOneTime = true;

                    createNewLine = true;

                    if (numberOfCreatedLines == 2)
                        aiStopCreateLines = true;
                    else
                        aiStopCreateLines = false;
                }

                if (buildingDificulty == AiOpponentDifficulty.Hard && !aiOneTime)
                {
                    aiTime = Time.time + 5f;
                    aiOneTime = true;

                    createNewLine = true;

                    if (numberOfCreatedLines == 3)
                        aiStopCreateLines = true;
                    else
                        aiStopCreateLines = false;
                }

                if (createNewLine && aiTime < Time.time && numberOfCreatedLines < numberOfLinesSupported &&
                    !aiStopCreateLines)
                {
                    OpponentAIController();

                    createNewLine = false;
                    aiOneTime = false;
                }
            }

            #endregion
        }
    }

    public void ResetBuilding()
    {
        numberOfCreatedLines = 0;
        aiStopCreateLines = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("TargetSprite") 
            && GameManager.Instance.currentSelectedBuilding.name.ToString() != this.gameObject.name.ToString())
        {
            if(!GameManager.Instance.currentSelectedBuilding.GetComponent<DrawPath>().isDragging && GameManager.Instance.currentPathLine != null)
               GameManager.Instance.currentPathLine.SetPosition(1,_highlightSprite.position);

            GameManager.Instance.insideBuild = true;
            GameManager.Instance.targetSelectedBuilding = this.gameObject;
            
            print("Enter YEP!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TargetSprite") 
            && GameManager.Instance.currentSelectedBuilding.name.ToString() != this.gameObject.name.ToString())
        {
            GameManager.Instance.insideBuild = false;
            
            print("Exit YEP!");
        }
    }

    void CloneBuildingBlock(Vector3 clonePosition)
    {
        Transform blockClone = Instantiate(block, clonePosition, Quaternion.identity);

        if(buildType == BuildType.Simple)
          blockClone.GetComponent<MeshRenderer>().material = topBlockMaterial;
        if(buildType == BuildType.Shield)
            blockClone.GetComponent<MeshRenderer>().material = topBlockMaterialShield;
        if(buildType == BuildType.Sword)
            blockClone.GetComponent<MeshRenderer>().material = topBlockMaterialSword;


        blockClone.parent = this.gameObject.transform;
        
        blocks.Add(blockClone);
    }

    void RemoveBuildingBlock()
    {
        GameObject currentBlock = blocks[blocks.Count - 1].gameObject;
        
        Destroy(currentBlock);
        
        blocks.RemoveAt(blocks.Count - 1);
    }

    void InitBuildingsWithBlocks()
    {
        int nOfBlocks = life / 6;

        blocks.Add(transform.GetChild(2).transform);

        for (int i = 0; i < nOfBlocks; i++)
            CloneBuildingBlock(new Vector3(blocks[blocks.Count-1].position.x,blocks[blocks.Count-1].position.y + dBetweenBlocks,blocks[blocks.Count-1].position.z));
    }

    void SetUpLineIdicatorsUI()
    {
        //Number of lines supported per building
        for (int i = 0; i < numberOfLinesSupported; i++)
            indicators[i].SetActive(true);
        for (int i = numberOfLinesSupported; i < 3; i++)
            indicators[i].SetActive(false);
        
        //Number of created lines per building
        for (int i = 0; i < numberOfCreatedLines; i++)
            indicators[i].GetComponent<Image>().color = buildingColor;
        for (int i = numberOfCreatedLines; i < 3; i++)
            indicators[i].GetComponent<Image>().color = Color.white;
    }

    public void TransformBuildingToWinnerColor(Type typeBlock, Material bMaterial, Material topMaterial,
        Color buildColor, Material bShieldMaterial,Material topShieldMaterial,Material bSwordMaterial,Material topSwordMaterial)
    {
        baseMaterial = bMaterial;
        topBlockMaterial = topMaterial;

        baseMaterialShield = bShieldMaterial;
        topBlockMaterialShield = topShieldMaterial;

        baseMaterialSword = bSwordMaterial;
        topBlockMaterialSword = topSwordMaterial;

        buildingColor = buildColor;
        type = typeBlock;
    }

    public void ChangeMaterials(BuildingController targetBuild,BuildingController t2)
    {
        if (t2.buildType == BuildType.Simple)
        {
            transform.GetChild(2).GetComponent<MeshRenderer>().material = targetBuild.baseMaterial;
            block.GetComponent<MeshRenderer>().material = targetBuild.topBlockMaterial;
        }

        if (t2.buildType == BuildType.Shield)
        {
            transform.GetChild(2).GetComponent<MeshRenderer>().material = targetBuild.baseMaterialShield;
            block.GetComponent<MeshRenderer>().material = targetBuild.topBlockMaterialShield;
        }

        if (t2.buildType == BuildType.Sword)
        {
            transform.GetChild(2).GetComponent<MeshRenderer>().material = targetBuild.baseMaterialSword;
            block.GetComponent<MeshRenderer>().material = targetBuild.topBlockMaterialSword;
        }
    }

    void OpponentAIController()
    {
        freeBuildings = FindFreeBuildingsToCreateLines();
        
        if(freeBuildings.Count > 0)
          CreateOpponentLine();
    }

    List<Transform> FindFreeBuildingsToCreateLines()
    {
        GameObject[] allBuildings = GameObject.FindGameObjectsWithTag("Building");
        List<Transform> avaiableBuildings = new List<Transform>();

        for (int i = 0; i < allBuildings.Length; i++)
        {
            if (allBuildings[i].name.ToString() != this.name.ToString())
                if (CheckIfBuildingNotBusy(allBuildings[i]))
                    avaiableBuildings.Add(allBuildings[i].transform);
        }

        return avaiableBuildings;
    }

    bool CheckIfBuildingNotBusy(GameObject build)
    {
        foreach (var obj in busyBuildings)
        {
            if (build.name.ToString() == obj.name.ToString() || build.GetComponent<BuildingController>().type == obj.GetComponent<BuildingController>().type)
                return false;
        }

        return true;
    }

    void CreateOpponentLine()
    {
        if (type == Type.Opponent)
            lineClone = Instantiate(linePrefab[0]);
        if (type == Type.Opponent2)
            lineClone = Instantiate(linePrefab[1]);
        
        lineClone.parent = GameObject.Find("GameplayEnviroment").transform;

        lineClone.GetComponent<LineRenderer>().startWidth = 0.04f;
        
        lineClone.GetComponent<LineRenderer>().SetPosition(0,new Vector3(transform.GetChild(4).transform.position.x,-1.408f,transform.GetChild(4).transform.position.z));
        lineClone.GetComponent<LineRenderer>().SetPosition(1,new Vector3(freeBuildings[freeBuildings.Count-1].GetChild(4).transform.position.x,-1.408f,freeBuildings[freeBuildings.Count-1].GetChild(4).transform.position.z));

        lineClone.GetComponent<LineProperties>().lineOrigin = this.gameObject.GetComponent<BuildingController>();
        lineClone.GetComponent<LineProperties>().lineTarget =
            freeBuildings[freeBuildings.Count - 1].GetComponent<BuildingController>();

        lineClone.GetComponent<LineProperties>().activateOffsetMoving = true;
        
        createdLines.Add(lineClone.gameObject);

        lineClone.GetComponent<LineRenderer>().enabled = false;
        
        busyBuildings.Add(freeBuildings[freeBuildings.Count - 1]);
        freeBuildings[freeBuildings.Count-1].gameObject.GetComponent<BuildingController>().busyBuildings.Add(this.transform);
        
        AddColliderToLine(lineClone.GetComponent<LineRenderer>(),new Vector3(transform.GetChild(4).transform.position.x,-1.408f,transform.GetChild(4).transform.position.z)
            ,new Vector3(freeBuildings[freeBuildings.Count-1].GetChild(4).transform.position.x,-1.408f,freeBuildings[freeBuildings.Count-1].GetChild(4).transform.position.z));

        numberOfCreatedLines++;
    }
    private void AddColliderToLine(LineRenderer line, Vector3 startPoint, Vector3 endPoint)
    {
        BoxCollider lineCollider = new GameObject("LineColliderOpponent").AddComponent<BoxCollider>();
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
    }

    void SetUpCanvasScaler()
    {
        transform.GetChild(3).GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        transform.GetChild(3).GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        transform.GetChild(3).GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
        transform.GetChild(3).GetComponent<CanvasScaler>().referenceResolution = new Vector2(1125,2436);
    }
}
