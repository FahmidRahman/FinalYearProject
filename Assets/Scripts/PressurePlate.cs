using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool isPressed = false;
    private PuzzleManager puzzleManager;

    private void Start()
    {
        puzzleManager = FindFirstObjectByType<PuzzleManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock") && !isPressed)
        {
            isPressed = true;
            puzzleManager.PlatePressed();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock") && isPressed)
        {
            isPressed = false;
            puzzleManager.PlateReleased();
        }
    }
}
