using UnityEngine;

public class DialogueTriggerZone1 : MonoBehaviour
{
    public NPCPlayerTrigger2 npcDialoguePatrol;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            npcDialoguePatrol.TriggerDialogueApproach();
        }
    }
}
