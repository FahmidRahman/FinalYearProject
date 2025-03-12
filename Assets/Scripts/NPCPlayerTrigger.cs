using System.Collections;
using UnityEngine;
using TMPro;

public class NPCPlayerTrigger : MonoBehaviour
{
    private enum NPCState { PATROL, APPROACHING, DIALOGUE, EXITING }
    private NPCState currentState = NPCState.PATROL;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float waitTimeAtPoint = 0.5f;

    private int currentPointIndex = 0;
    private bool waiting = false;
    private bool movingForward = true;

    [Header("Dialogue Settings")]
    public string[] dialogueLines;
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    private int dialogueIndex = 0;

    [Header("Special Positions")]
    public Transform exitPoint;
    private Vector2 dynamicDialoguePosition; // Dynamic position next to the player

    [Header("Animation Settings")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Transform playerTransform;
    private Animator playerAnimator;
    private bool playerFrozenExternally = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }
        else
        {
            Debug.LogWarning("No patrol points assigned to NPC!");
        }

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerAnimator = player.GetComponent<Animator>(); // Get player's animator
        }
    }

    void Update()
    {

        
        switch (currentState)
        {
            case NPCState.PATROL:
                Patrol();
                break;
            case NPCState.APPROACHING:
                Approach();
                break;
            case NPCState.DIALOGUE:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    AdvanceDialogue();
                }
                break;
            case NPCState.EXITING:
                Exit();
                break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0 || waiting)
            return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        UpdateAnimation(direction);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(WaitAndMoveToNext());
        }
    }

    IEnumerator WaitAndMoveToNext()
    {
        waiting = true;
        UpdateAnimation(Vector2.zero);
        yield return new WaitForSeconds(waitTimeAtPoint);

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

    void Approach()
    {
        Vector2 direction = (dynamicDialoguePosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, dynamicDialoguePosition, moveSpeed * Time.deltaTime);
        UpdateAnimation(direction);

        if (Vector2.Distance(transform.position, dynamicDialoguePosition) < 0.1f)
        {
            currentState = NPCState.DIALOGUE;
            StartDialogue();
        }
    }

    void Exit()
    {
        if (exitPoint == null)
        {
            Debug.LogError("Exit point not set!");
            return;
        }

        Vector2 direction = ((Vector2)exitPoint.position - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, exitPoint.position, moveSpeed * Time.deltaTime);
        UpdateAnimation(direction);

        if (Vector2.Distance(transform.position, exitPoint.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    void UpdateAnimation(Vector2 direction)
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);

        if (direction.magnitude < 0.1f)
        {
            return;
        }

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetBool("isWalking", true);
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
            spriteRenderer.flipX = false;
        }
    }

    public void TriggerDialogueApproach()
    {
        if (currentState == NPCState.PATROL)
        {
            FreezePlayer(true);
            CalculateDynamicDialoguePosition();
            currentState = NPCState.APPROACHING;
        }
    }

    void CalculateDynamicDialoguePosition()
    {
        if (playerTransform != null)
        {
            float smallOffset = 0.18f; // Small offset to the left
            dynamicDialoguePosition = new Vector2(playerTransform.position.x - smallOffset, playerTransform.position.y);
        }
    }


    void StartDialogue()
    {
        dialogueIndex = 0;
        UpdateAnimation(Vector2.zero); // NPC stands idle during dialogue

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
            dialogueText.text = dialogueLines[dialogueIndex];
        }
    }

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

    void EndDialogue()
    {
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }

        StartCoroutine(UnfreezePlayerAfterDelay(2.5f)); // Freeze player for 2.5 seconds
        currentState = NPCState.EXITING;
    }

    void FreezePlayer(bool freeze)
    {
        if (playerTransform != null)
        {
            var playerController = playerTransform.GetComponent<PlayerController>();
            var playerAnimator = playerTransform.GetComponent<Animator>();

            if (playerController != null)
            {
                playerController.enabled = !freeze; // Disable movement when frozen
            }

            if (playerAnimator != null)
            {
                if (freeze)
                {
                    playerAnimator.SetBool("isWalking", false);
                    playerAnimator.SetBool("isWalkingUp", false);
                    playerAnimator.SetBool("isWalkingDown", false);
                    playerAnimator.Play("player_idle");
                }
            }
        }
    }


    IEnumerator UnfreezePlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!playerFrozenExternally)
        {
            FreezePlayer(false);
        }
    }
}
