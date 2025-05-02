using UnityEngine;
using TMPro;

public class SimpleNPCDialogueTMP : MonoBehaviour
{
    public GameObject dialogueUI;             // Assign the dialogue panel in the inspector
    public TextMeshProUGUI dialogueText;      // Assign the TMP text component
    public string message = "You shouldn't go this way right now."; // Custom message

    private bool isPlayerInRange = false;
    private bool dialogueActive = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDialogue();
        }
    }

    void ToggleDialogue()
    {
        dialogueActive = !dialogueActive;
        dialogueUI.SetActive(dialogueActive);
        dialogueText.text = dialogueActive ? message : "";

        // Optional: freeze player movement during dialogue
        if (dialogueActive)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (dialogueActive)
                ToggleDialogue();
        }
    }
}
