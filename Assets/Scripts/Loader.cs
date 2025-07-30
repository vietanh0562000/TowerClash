using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    void Start()
    {
        int lv = PlayerPrefs.GetInt("SavedLevel");

        if (lv == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(lv);
    }
}
