using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public VectorValue playerStorage;
    public Vector2 startPosition;
    public int targetSceneIndex = 1;

    public void StartGame()
    {
        playerStorage.initialValue = startPosition;
        SceneManager.LoadScene(targetSceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
