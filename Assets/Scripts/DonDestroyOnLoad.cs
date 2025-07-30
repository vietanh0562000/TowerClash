using UnityEngine;

public class DonDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
