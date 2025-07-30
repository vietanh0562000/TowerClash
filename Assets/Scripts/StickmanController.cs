using UnityEngine;

public class StickmanController : MonoBehaviour
{
    public enum StickmanType
    {
        Player,
        Opponent
    };

    [Header("Stickman Type")] public StickmanType type;

    [Header("Target Points")] 
    public Transform startPoint;
    public Transform targetPoint;

    [Header("Speed Movement")] [SerializeField]
    private float speed;

    void Start()
    {
        transform.position = new Vector3(startPoint.position.x, -1.4f, startPoint.position.z);
        
        Destroy(this.gameObject,10f);
    }

    void Update()
    {
        MoveStickman();
    }

    void MoveStickman()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(targetPoint.position.x, transform.position.y, targetPoint.position.z), step);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.14f && targetPoint.GetComponent<BuildingController>().life < 65)
        {
            if (targetPoint.GetComponent<BuildingController>().type == BuildingController.Type.Player &&
                startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Player ||
                targetPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent &&
                startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent ||                 
                targetPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent2 &&
                startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent2)
            {
                if (startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Simple || startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Sword)
                {
                    targetPoint.GetComponent<BuildingController>().life++;

                    GameManager.Instance.totalPoints++;
                }

                if (startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Shield)
                {
                    targetPoint.GetComponent<BuildingController>().life += 2;
                    
                    GameManager.Instance.totalPoints += 2;
                }

                if (targetPoint.GetComponent<BuildingController>().type == BuildingController.Type.Player &&
                    startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Player)
                {
                    if (startPoint.GetComponent<BuildingController>().buildType ==
                        BuildingController.BuildType.Simple ||
                        startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Sword)
                    {
                        GameManager.Instance.playerPoints++;
                    }

                    if (startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Shield)
                    {
                        GameManager.Instance.playerPoints += 2;
                    }
                }
            }

            else
            {
                targetPoint.GetComponent<BuildingController>().life--;
                GameManager.Instance.totalPoints--;

                if (startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Sword)
                {
                    targetPoint.GetComponent<BuildingController>().life--;
                    GameManager.Instance.totalPoints--;
                }

                if (targetPoint.GetComponent<BuildingController>().type == BuildingController.Type.Player &&
                    startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent ||
                    targetPoint.GetComponent<BuildingController>().type == BuildingController.Type.Player &&
                    startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent2)
                { 
                    if (startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Simple || startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Shield)
                    {
                        GameManager.Instance.playerPoints--;
                    }
                    if (startPoint.GetComponent<BuildingController>().buildType == BuildingController.BuildType.Sword)
                    {
                        GameManager.Instance.playerPoints -= 2;
                    }
                }

                if (targetPoint.GetComponent<BuildingController>().life < 0)
                {
                    targetPoint.GetComponent<BuildingController>().TransformBuildingToWinnerColor(
                        startPoint.GetComponent<BuildingController>().type,
                        startPoint.GetComponent<BuildingController>().baseMaterial
                        , startPoint.GetComponent<BuildingController>().topBlockMaterial,
                        startPoint.GetComponent<BuildingController>().buildingColor,startPoint.GetComponent<BuildingController>().baseMaterialShield,
                        startPoint.GetComponent<BuildingController>().topBlockMaterialShield,startPoint.GetComponent<BuildingController>().baseMaterialSword,
                        startPoint.GetComponent<BuildingController>().topBlockMaterialSword);

                    targetPoint.GetComponent<BuildingController>()
                        .ChangeMaterials(startPoint.GetComponent<BuildingController>(),targetPoint.GetComponent<BuildingController>());

                    if(targetPoint.GetComponent<BuildingController>().createdLines.Count > 0)
                      RemoveLines(targetPoint);
                    
                    int rand = Random.Range(0, 3);

                    if (rand < 2 && startPoint.GetComponent<BuildingController>().createdLines.Count > 0 && (startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent ||
                        startPoint.GetComponent<BuildingController>().type == BuildingController.Type.Opponent2))
                    {
                        RemoveLines(startPoint);
                        startPoint.GetComponent<BuildingController>().ResetBuilding();
                    }

                    targetPoint.GetComponent<BuildingController>().ResetBuilding();

                    targetPoint.GetComponent<BuildingController>().life = 0;
                }
            }


            AudioManager.Instance.StickmanClip();
            Destroy(this.gameObject);
        }
    }
    void RemoveLines(Transform target)
    {
        for (int i = 0; i < target.GetComponent<BuildingController>().createdLines.Count; i++)
        {
            GameObject ln = target.GetComponent<BuildingController>().createdLines[i];
            
            target.GetComponent<BuildingController>().createdLines
                .Remove(target.GetComponent<BuildingController>().createdLines[i]);

            Destroy(ln);
        }
    }
}
