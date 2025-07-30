using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Sound Clips")] 
    [SerializeField] private AudioClip rightBuilding;
    [SerializeField] private AudioClip wrongBuilding;
    [SerializeField] private AudioClip stickmanClip;
    [SerializeField] private AudioClip removeLine;

    private AudioSource soundManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    void Start()
    {
        soundManager = GetComponent<AudioSource>();
    }

    public void RightBuilding()
    {
        soundManager.clip = rightBuilding;
        soundManager.Play();
    }

    public void WrongBuilding()
    {
        soundManager.clip = wrongBuilding;
        soundManager.Play();
    }

    public void StickmanClip()
    {
        if (!soundManager.isPlaying)
        {
            soundManager.clip = stickmanClip;
            soundManager.Play();
        }
    }

    public void RemoveLine()
    {
        soundManager.clip = removeLine;
        soundManager.Play();
    }
}
