using UnityEngine;
using TMPro;

public class Orb : MonoBehaviour
{
    public GameObject dialogueBox;          // UI box that shows the dialogue
    public TMP_Text dialogueText;           // The TMP text field inside the dialogue box
    [TextArea] public string dialogue = "You have claimed the Orb.";

    public GameObject orb;                  // The visible orb (active at start)
    public GameObject orbTaken;             // The empty or claimed version (inactive at start)

    private bool playerInRange = false;     // Tracks if player is near the orb
    private bool orbClaimed = false;        // Prevents claiming more than once

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (!orbClaimed)
            {
                if (!dialogueBox.activeInHierarchy)
                {
                    dialogueBox.SetActive(true);
                    dialogueText.text = dialogue;

                    orb.SetActive(false);
                    orbTaken.SetActive(true);
                    orbClaimed = true;
                }
                else
                {
                    dialogueBox.SetActive(false);
                }
            }
            else
            {
                dialogueBox.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player entered orb trigger.");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("❌ Player exited orb trigger.");
            playerInRange = false;
            dialogueBox.SetActive(false);
        }
    }
}
