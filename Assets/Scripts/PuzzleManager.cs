using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int totalPlates = 4;
    private int currentPressed = 0;

    public GameObject doorLocked;
    public GameObject doorUnlocked;

    public void PlatePressed()
    {
        currentPressed++;

        if (currentPressed == totalPlates)
        {
            Debug.Log("Puzzle complete! Door opened.");

            if (doorLocked != null) doorLocked.SetActive(false);
            if (doorUnlocked != null) doorUnlocked.SetActive(true);
        }
    }

    public void PlateReleased()
    {
        currentPressed--;

        // Optional: revert if a rock is removed
        if (currentPressed < totalPlates)
        {
            if (doorLocked != null) doorLocked.SetActive(true);
            if (doorUnlocked != null) doorUnlocked.SetActive(false);

            Debug.Log("Plate released. Door closed again.");
        }
    }
}
