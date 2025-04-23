using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public string requiredRockID = "A";
    public bool isPressed = false;
    public PuzzleManager puzzleManager; // Manually assigned in Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock"))
        {
            PushableRock rock = collision.GetComponent<PushableRock>();
            if (rock != null && rock.rockID == requiredRockID && !isPressed)
            {
                isPressed = true;
                puzzleManager.PlatePressed();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock"))
        {
            PushableRock rock = collision.GetComponent<PushableRock>();
            if (rock != null && rock.rockID == requiredRockID && isPressed)
            {
                isPressed = false;
                puzzleManager.PlateReleased();
            }
        }
    }
}
