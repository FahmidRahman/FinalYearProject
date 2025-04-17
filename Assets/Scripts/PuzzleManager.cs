using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PressurePlate[] pressurePlates;
    public GameObject doorToDestroy;

    void Update()
    {
        if (AllPlatesActivated())
        {
            doorToDestroy.SetActive(false); // Or use Destroy(doorToDestroy);
            enabled = false; // Stop checking once done
        }
    }

    bool AllPlatesActivated()
    {
        foreach (PressurePlate plate in pressurePlates)
        {
            if (!plate.isActivated)
                return false;
        }
        return true;
    }
}
