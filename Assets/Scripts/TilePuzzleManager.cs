using System.Collections.Generic;
using UnityEngine;

public class TilePuzzleManager : MonoBehaviour
{
    private List<PuzzleTile> puzzleTiles = new List<PuzzleTile>();

    public GameObject lockedDoor;
    public GameObject unlockedDoor;

    private void Start()
    {
        // Auto-register all child PuzzleTiles
        foreach (Transform child in transform)
        {
            PuzzleTile tile = child.GetComponent<PuzzleTile>();
            if (tile != null)
            {
                puzzleTiles.Add(tile);
                tile.AssignManager(this); // let the tile know who its manager is
            }
        }

        if (unlockedDoor != null)
            unlockedDoor.SetActive(false);
    }

    public void ResetPuzzle()
    {
        foreach (PuzzleTile tile in puzzleTiles)
            tile.ResetTile();

        Debug.Log("Puzzle reset for group: " + gameObject.name);
    }

    public void CheckForCompletion()
    {
        foreach (PuzzleTile tile in puzzleTiles)
        {
            if (!tile.IsActivated())
                return;
        }

        Debug.Log("Puzzle complete for group: " + gameObject.name);
        SwapDoors();
    }

    private void SwapDoors()
    {
        if (lockedDoor != null) lockedDoor.SetActive(false);
        if (unlockedDoor != null) unlockedDoor.SetActive(true);
    }
}
