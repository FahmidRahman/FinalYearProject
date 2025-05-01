using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
            musicSource.volume = savedVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
