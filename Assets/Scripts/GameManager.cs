using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Menu,
        Gameplay,
        Win,
        Loose,
        Tutorial,
    };
    
    public static GameManager Instance;

    [Header("Game State")] 
    public GameState gameState;

    [Header("Prefabs")] 
    public LineRenderer linePrefab;

    [Header("Trails")]
    public Transform fingerTrail;
    
    [Header("Materials")] 
    public Material blueMaterial;
    public Material redMaterial;
    public Material movingLinePath;

    [Header("Textures")]
    public Texture movTex;

    [Header("Level Stats")] 
    public int levelNumber;
    public int playerPoints;
    public int totalPoints;
    public bool winner;
    public bool looser;
    public int levelReward;
    public int coins;

    [Header("Animator Controllers")] 
    public Animator cameraController;

    [Header("Enviromets")] 
    [SerializeField] private GameObject menuEnvir;
    [SerializeField] private GameObject gameplayEnvir;
    [SerializeField] private GameObject cameraGame;
    [SerializeField] private GameObject cameraMenu;

    [Header("Effects")] [SerializeField] 
    private ParticleSystem confetti;

    [Header("Tutorials")] 
    [SerializeField] private GameObject tutorial1;
    [SerializeField] private GameObject tutorial2;
    [SerializeField] private GameObject tutorialSword;
    [SerializeField] private GameObject tutorialShield;
    
    [HideInInspector] public GameObject currentSelectedBuilding;
    [HideInInspector] public List<GameObject> avaiableLines;
    [HideInInspector] public GameObject targetSelectedBuilding;
    [HideInInspector] public LineRenderer currentPathLine;
    
    [HideInInspector] public bool insideBuild;
    [HideInInspector] public bool isDragging;

    private bool oneTimeConffeti;
    private bool stopChecking;
    
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        fingerTrail.gameObject.SetActive(false);

        coins = PlayerPrefs.GetInt("Coins");
        levelNumber = PlayerPrefs.GetInt("lvNumber");

        if (levelNumber == 0)
        {
            levelNumber = 1;
            
            PlayerPrefs.SetInt("lvNumber",1);
            PlayerPrefs.Save();
        }
        
        Advertisements.Instance.Initialize();
        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM,BannerType.SmartBanner);
        
        UIManager.Instance.UpdateUi();
    }
    void Update()
    {
        if (gameState == GameState.Gameplay)
        {
            if (!isDragging)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MoveFingerTrail();
                    fingerTrail.gameObject.SetActive(true);
                }

                if (Input.GetMouseButton(0))
                    MoveFingerTrail();
                if (Input.GetMouseButtonUp(0))
                    fingerTrail.gameObject.SetActive(false);
            }

            winner = CheckWinner();
            looser = CheckLooser();
        }

        if (winner && !stopChecking)
            StartCoroutine(CompleteLevel());
        if (looser && !stopChecking)
            LevelFail();
    }

    public void PauseGame(Action eventt)
    {
        Time.timeScale = 0;
        var coin = PlayerPrefs.GetInt("Coins");
        if (coin > 100)
        {
            PlayerPrefs.SetInt("Coins",coin - 100);
        }
        else
        {
            eventt?.Invoke();
        }
       
        StartCoroutine(TimeoutExample());
    }
    
    IEnumerator TimeoutExample()
    {
        Debug.Log("Start timeout");
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1;
    }
    public void CreateLine()
    {
        currentPathLine = Instantiate(linePrefab);
        
        currentPathLine.transform.parent = GameObject.Find("GameplayEnviroment").transform;

        currentPathLine.startWidth = 0.05f;
    }

    public bool CheckIfLineExist(string lineName)
    {
        foreach (var line in avaiableLines)
        {
            if (line != null)
            {
                if (lineName == line.name.ToString())
                    return true;
            }
        }

        return false;
    }

    void MoveFingerTrail()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitPoint;

        if (Physics.Raycast(ray, out hitPoint, 100f))
        {
            fingerTrail.position = new Vector3(hitPoint.point.x,fingerTrail.position.y,hitPoint.point.z);
        }
    }

    bool CheckWinner()
    {
        BuildingController[] buildings = GameObject.FindObjectsOfType<BuildingController>();

        foreach (var obj in buildings)
        {
            if (obj.type == BuildingController.Type.Opponent || obj.type == BuildingController.Type.Neutral || obj.type ==  BuildingController.Type.Opponent2)
                return false;
        }

        return true;
    }

    bool CheckLooser()
    {
        BuildingController[] buildings = GameObject.FindObjectsOfType<BuildingController>();

        foreach (var obj in buildings)
        {
            if (obj.type == BuildingController.Type.Player)
                return false;
        }

        return true;
    }

    public void Play()
    {
        cameraController.SetBool("Gameplay",true);
        cameraController.SetBool("Menu", false);
        
        UIManager.Instance.menu.SetActive(false);
        UIManager.Instance.gameplay.SetActive(true);

        GameObject.Find("Replay_btn").AddComponent<ReplayBtn>();
        
        gameplayEnvir.SetActive(true);
        menuEnvir.SetActive(false);
        cameraGame.SetActive(true);
        cameraMenu.SetActive(false);

        if (levelNumber == 1)
        {
            stopChecking = true;
            GameManager.Instance.gameState = GameState.Tutorial;
            StartCoroutine(ShowTutorial());
        }

        if(levelNumber > 1)
          GameManager.Instance.gameState = GameState.Gameplay;
        
        if(SceneManager.GetActiveScene().name == "Level11")
            TutorialSword();
        if(SceneManager.GetActiveScene().name == "Level9")
            TutorialShield();
    }

    IEnumerator ShowTutorial()
    {
        yield return new WaitForSeconds(0.85f);
        
        tutorial1.SetActive(true);
    }
    public void NextLevel()
    {
        coins += levelReward;
        levelNumber++;
        
        PlayerPrefs.SetInt("lvNumber",levelNumber);
        PlayerPrefs.SetInt("Coins",coins);
        PlayerPrefs.Save();

        if (SceneManager.GetActiveScene().buildIndex == 20)
        {
            PlayerPrefs.SetInt("SavedLevel",3);
            PlayerPrefs.Save();
            
            SceneManager.LoadScene(3);
        }
        else
        {
            PlayerPrefs.SetInt("SavedLevel",SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.Save();
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public IEnumerator CompleteLevel()
    {
        if (!oneTimeConffeti)
        {
            stopChecking = true;
            gameplayEnvir.SetActive(false);
            confetti.Play();
            oneTimeConffeti = true;
        }

        yield return new WaitForSeconds(0.65f);
        
        Advertisements.Instance.ShowRewardedVideo(CompleteMethod);

        GameManager.Instance.gameState = GameState.Win;
        
        UIManager.Instance.background.SetActive(true);
        UIManager.Instance.lvCompletePannel.enabled = true;
        UIManager.Instance.lvCompleteReward.text = "Reward:      " + levelReward;
    }
    public void LevelFail()
    {
        GameManager.Instance.gameState = GameState.Loose;
        
        UIManager.Instance.background.SetActive(true);
        UIManager.Instance.lvFailPannel.enabled = true;
        UIManager.Instance.lvCompleteReward.text = "Reward:      " + 0;
        
        Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
        
        gameplayEnvir.SetActive(false);
    }

    private void CompleteMethod(bool completed, string advertiser)
    {
        Debug.Log("Closed rewarded from: " + advertiser + " -> Completed " + completed);
        if (completed == true)
        {
            print("Ads is shown!");
        }
        else
        {
            print("Ads not shown!");
        }
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Tutorial1()
    {
        tutorial1.SetActive(false);
        tutorial2.SetActive(true);
    }

    public void Tutorial2()
    {
        tutorial2.SetActive(false); 
        gameplayEnvir.SetActive(true);
        stopChecking = false;

        GameManager.Instance.gameState = GameState.Gameplay;
    }

    public void TutorialSword()
    {
        tutorialSword.SetActive(true);
    }
    
    public void TutorialShield()
    {
        tutorialShield.SetActive(true);
    }

    public void CloseTutorialShield()
    {
        tutorialShield.SetActive(false);
    }
    public void CloseTutorialSword()
    {
        tutorialSword.SetActive(false);
    }
}
