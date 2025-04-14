using System.Collections;
using UnityEngine;
using TMPro;

public class NPCDialoguePatrol : MonoBehaviour
{
    // ------------------------------
    // PATROLLING VARIABLES
    // ------------------------------
    [Header("Patrol Settings")]
    [Tooltip("Ordered waypoints for the NPC to patrol (e.g., A, B, C, D)")]
    public Transform[] patrolPoints;
    
    [Tooltip("Movement speed of the NPC.")]
    public float moveSpeed = 2f;
    
    [Tooltip("Time the NPC waits at each waypoint before moving on.")]
    public float waitTimeAtPoint = 0.5f;
    
    private int currentPointIndex = 0;
    private bool waiting = false;
    private bool movingForward = true; // true: forward A->B->C->D; false: reverse back
    
    // ------------------------------
    // DIALOGUE VARIABLES
    // ------------------------------
    [Header("Dialogue Settings")]
    [Tooltip("Array of dialogue lines to show when interacting with the NPC.")]
    public string[] dialogueLines;
    
    [Tooltip("The dialogue box GameObject (should be disabled by default).")]
    public GameObject dialogueBox;
    
    [Tooltip("The TMP_Text component to display dialogue text.")]
    public TMP_Text dialogueText;
    
    private int dialogueIndex = 0;
    private bool playerInDialogueRange = false;
    private bool inDialogue = false;
    
    // ------------------------------
    // COMPONENT REFERENCES
    // ------------------------------
    [Header("Animation Settings")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Optional: Reference to the player so the NPC can face them when dialogue starts
    private Transform playerTransform;
    
    void Start()
    {
        // Cache components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Initialize NPC position to the first waypoint (if available)
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }
        else
        {
            Debug.LogWarning("No patrol points assigned to NPCDialoguePatrol!");
        }
        
        // Ensure the dialogue box is hidden initially
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
        
        // Optionally, find the player GameObject (ensure the player is tagged "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    void Update()
    {



        // --- Dialogue Mode ---
        // If we're already in dialogue, check for the E key to advance dialogue.
        if (inDialogue)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AdvanceDialogue();
            }
            // Do not run patrolling code while in dialogue.
            return;
        }
        
        // --- Initiate Dialogue ---
        // If the player is in range and presses E, start the dialogue.
        if (playerInDialogueRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
            return; // skip patrolling until dialogue is done
        }
        
        // --- Patrolling Mode ---
        Patrol();
    }
    
    // ------------------------------
    // PATROLLING METHODS
    // ------------------------------
    void Patrol()
    {
        if (patrolPoints.Length == 0 || waiting)
            return;
        
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 direction = ((Vector2)targetPoint.position - (Vector2)transform.position).normalized;
        
        // Move the NPC toward the target waypoint.
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        
        // Update animation based on the movement direction.
        UpdateAnimation(direction);
        
        // When close enough to the waypoint, wait briefly before moving to the next one.
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(WaitAndMoveToNext());
        }
    }
    
    IEnumerator WaitAndMoveToNext()
    {
        waiting = true;
        // Stop movement animation.
        UpdateAnimation(Vector2.zero);
        
        yield return new WaitForSeconds(waitTimeAtPoint);
        
        // Update the current waypoint index for back-and-forth movement.
        if (movingForward)
        {
            if (currentPointIndex < patrolPoints.Length - 1)
                currentPointIndex++;
            else
            {
                movingForward = false;
                currentPointIndex--;
            }
        }
        else
        {
            if (currentPointIndex > 0)
                currentPointIndex--;
            else
            {
                movingForward = true;
                currentPointIndex++;
            }
        }
        waiting = false;
    }
    
    /// <summary>
    /// Updates the NPC’s animations based on movement direction.
    /// Uses three booleans: isWalking (horizontal), isWalkingUp, isWalkingDown.
    /// Flips the sprite if moving left.
    /// </summary>
    /// <param name="direction">Normalized direction vector.</param>
    void UpdateAnimation(Vector2 direction)
    {
        // Reset all animation booleans.
        animator.SetBool("isWalking", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
        
        // If there is little movement, leave all booleans false.
        if (direction.magnitude < 0.1f)
        {
            return;
        }
        
        // Determine if horizontal or vertical movement is dominant.
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetBool("isWalking", true);
            // Default horizontal animation faces right. Flip if moving left.
            spriteRenderer.flipX = (direction.x < 0);
        }
        else
        {
            if (direction.y > 0)
            {
                animator.SetBool("isWalkingUp", true);
            }
            else
            {
                animator.SetBool("isWalkingDown", true);
            }
            // For vertical movement, assume no horizontal flipping is needed.
            spriteRenderer.flipX = false;
        }
    }
    
    // ------------------------------
    // DIALOGUE METHODS
    // ------------------------------
    /// <summary>
    /// Called when the player enters the NPC’s dialogue range.
    /// Sets a flag to allow dialogue initiation.
    /// </summary>
    /// <param name="collision">The collider that entered.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInDialogueRange = true;
        }
    }
    
    /// <summary>
    /// Called when the player exits the NPC’s dialogue range.
    /// Clears the flag and optionally ends dialogue if in progress.
    /// </summary>
    /// <param name="collision">The collider that exited.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInDialogueRange = false;
            // Optionally, you might end dialogue if the player walks away.
            // EndDialogue();
        }
    }
    
    /// <summary>
    /// Starts the dialogue sequence.
    /// Pauses patrolling, shows the dialogue UI, and displays the first line.
    /// </summary>
    void StartDialogue()
    {
        inDialogue = true;
        dialogueIndex = 0;
        
        animator.SetBool("isWalking", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
        
        // disable player movement whilst speaking to the NPC
        if (playerTransform != null)
        {
            var playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }
        
        // Show the dialogue UI and text can be customisable within unity inspector
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
            dialogueText.text = dialogueLines[dialogueIndex];
        }
    }
    
    /// <summary>
    /// Advances to the next dialogue line.
    /// If there are no more lines, ends the dialogue.
    /// </summary>
    void AdvanceDialogue()
    {
        dialogueIndex++;
        if (dialogueIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[dialogueIndex];
        }
        else
        {
            EndDialogue();
        }
    }
    
    /// <summary>
    /// Ends the dialogue, hides the dialogue UI, and resumes NPC patrolling.
    /// </summary>
    void EndDialogue()
    {
        inDialogue = false;
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }

        // Re-enable the player's movement by enabling their PlayerController.
        if (playerTransform != null)
        {
            var playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }
    
    /// <summary>
    /// Makes the NPC face toward the player.
    /// Adjusts the NPC’s animation parameters accordingly.
    /// </summary>
    void FacePlayer()
    {
        if (playerTransform == null)
            return;
        
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // Use similar logic to UpdateAnimation to determine facing.
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal: Use the walking animation with sprite flipping.
            animator.SetBool("isWalking", true);
            spriteRenderer.flipX = (direction.x < 0);
        }
        else
        {
            // Vertical: set the appropriate vertical boolean.
            if (direction.y > 0)
                animator.SetBool("isWalkingUp", true);
            else
                animator.SetBool("isWalkingDown", true);
        }
    }
}
