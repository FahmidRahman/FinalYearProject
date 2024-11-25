using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sign : MonoBehaviour
{

    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public string dialogue;
    public bool playerInRange;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            Debug.Log("Player in range");
            playerInRange = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            Debug.Log("Player not in range");
            playerInRange = false;
            dialogueBox.SetActive(false);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerInRange) {
            if(dialogueBox.activeInHierarchy) {
                dialogueBox.SetActive(false);
            }
            else {
                dialogueBox.SetActive(true);
                dialogueText.text = dialogue;
                Debug.Log("Dialogue set to: " + dialogueText.text);
            }
        }
    }
}
