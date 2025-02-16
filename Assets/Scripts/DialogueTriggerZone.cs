using UnityEngine;

public class DialogueTriggerZone : MonoBehaviour
{
    public NPCPlayerTrigger npcDialoguePatrol; // Assign this in the Inspector.
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            npcDialoguePatrol.TriggerDialogueApproach();
        }
    }
}
