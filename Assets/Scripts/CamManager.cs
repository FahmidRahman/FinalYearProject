using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }
}
