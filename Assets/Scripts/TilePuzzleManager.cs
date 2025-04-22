using System.Collections.Generic;
using UnityEngine;

public class TilePuzzleManager : MonoBehaviour
{
    private List<PuzzleTile> puzzleTiles = new List<PuzzleTile>();

    public GameObject lockedDoor;   // Drag in the locked door object
    public GameObject unlockedDoor; // Drag in the unlocked door object

    public void RegisterTile(PuzzleTile tile)
    {
        if (!puzzleTiles.Contains(tile))
            puzzleTiles.Add(tile);
    }

    public void ResetPuzzle()
    {
        foreach (PuzzleTile tile in puzzleTiles)
        {
            tile.ResetTile();
        }
        Debug.Log("Puzzle reset!");
    }

    public void CheckForCompletion()
    {
        foreach (PuzzleTile tile in puzzleTiles)
        {
            if (!tile.IsActivated())
                return;
        }

        Debug.Log("Puzzle complete!");
        SwapDoors();
    }

    private void SwapDoors()
    {
        if (lockedDoor != null) lockedDoor.SetActive(false);
        if (unlockedDoor != null) unlockedDoor.SetActive(true);
    }
}
