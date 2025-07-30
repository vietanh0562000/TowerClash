using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("UI Elements")] 
    [SerializeField] private Image contentBar;
    public GameObject menu;
    public GameObject gameplay;
    public Text lvCompleteReward;
    public Text menuCoinsText;
    public Text lvNumber;
    public GameObject background;

    [Header("Animators")] 
    public Animator lvCompletePannel;
    public Animator lvFailPannel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    void Update()
    {
        UpdateUi();
    }

    public void UpdateUi()
    {
        float fillAmountBar = (float)GameManager.Instance.playerPoints / GameManager.Instance.totalPoints;

        contentBar.fillAmount = fillAmountBar;

        menuCoinsText.text = "" + GameManager.Instance.coins;
        lvNumber.text = "LEVEL " + GameManager.Instance.levelNumber;
    }
}
