using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayBtn : MonoBehaviour
{
    public Button yourButton;

    void Start () {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
