using UnityEngine;
using System.Collections;

public class NPCPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Ordered waypoints for the NPC to patrol (e.g., A, B, C, D)")]
    public Transform[] patrolPoints;
    
    [Tooltip("Movement speed of the NPC.")]
    public float moveSpeed = 2f;
    
    [Tooltip("Time the NPC waits at each waypoint before moving on.")]
    public float waitTimeAtPoint = 0.5f;

    private int currentPointIndex = 0;
    private bool waiting = false;
    private bool movingForward = true; // When true: A -> B -> C -> D; when false: reverse

    [Header("Animation Settings")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;  // Needed for flipping the sprite

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start at the first waypoint (if available)
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }
        else
        {
            Debug.LogWarning("No patrol points assigned to NPCPatrol!");
        }
    }

    void Update()
    {
        if (patrolPoints.Length == 0 || waiting)
            return;

        Transform targetPoint = patrolPoints[currentPointIndex];

        // Calculate the direction vector toward the target waypoint.
        Vector2 direction = ((Vector2)targetPoint.position - (Vector2)transform.position).normalized;

        // Move the NPC toward the target point.
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Update animations based on the direction of movement.
        UpdateAnimation(direction);

        // If close enough to the waypoint, start a waiting coroutine before moving on.
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(WaitAndMoveToNext());
        }
    }

    IEnumerator WaitAndMoveToNext()
    {
        waiting = true;
        // Reset animations when stopping (i.e., no movement)
        UpdateAnimation(Vector2.zero);
        yield return new WaitForSeconds(waitTimeAtPoint);

        // Determine the next waypoint index based on our patrol direction.
        if (movingForward)
        {
            if (currentPointIndex < patrolPoints.Length - 1)
                currentPointIndex++;
            else
            {
                movingForward = false; // reverse direction
                currentPointIndex--;
            }
        }
        else
        {
            if (currentPointIndex > 0)
                currentPointIndex--;
            else
            {
                movingForward = true; // change direction again
                currentPointIndex++;
            }
        }
        waiting = false;
    }

    /// <summary>
    /// Updates the Animator booleans for walking animations based on movement direction.
    /// - For horizontal movement, it sets isWalking (and flips the sprite if moving left).
    /// - For vertical movement, it sets isWalkingUp (if moving up) or isWalkingDown (if moving down).
    /// </summary>
    /// <param name="direction">Normalized direction vector of movement.</param>
    void UpdateAnimation(Vector2 direction)
    {
        // Reset all animation booleans.
        animator.SetBool("isWalking", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);

        // If there's almost no movement, don't set any booleans.
        if (direction.magnitude < 0.1f)
        {
            return;
        }

        // Compare absolute horizontal and vertical movement.
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement takes precedence.
            animator.SetBool("isWalking", true);

            // If moving left, flip the sprite. Otherwise, keep default (right-facing).
            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            // Vertical movement is dominant.
            if (direction.y > 0)
            {
                animator.SetBool("isWalkingUp", true);
            }
            else
            {
                animator.SetBool("isWalkingDown", true);
            }
            // For vertical animations, we assume the sprite does not need flipping.
            spriteRenderer.flipX = false;
        }
    }
}
