using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    public Transform player;        // The player to teleport
    public Transform resetTarget;   // The GameObject to teleport the player to

    public void ResetPosition()
    {
        if (player != null && resetTarget != null)
        {
            player.position = resetTarget.position;
        }
    }
}
