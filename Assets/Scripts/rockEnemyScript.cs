using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockEnemyScript : Enemy
{
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform spawnPosition;

    // Knockback settings
    public float knockbackDuration = 0.2f; 
    public float knockbackForce = 5f;
    private bool isKnocked = false;

    private Rigidbody2D rb;
    private Animator animator;
    private bool hasIdled = false; // flag to trigger idle animation only once when player enters the chase radius

    void Start() {
        target = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Start with the enemy asleep.
        animator.SetBool("isAsleep", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalkingX", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
    }

    void Update() {
        // Only chase if not currently being knocked back
        if (!isKnocked) {
            CheckDistance();
        }
    }

    void CheckDistance() {
        float distance = Vector3.Distance(target.position, transform.position);

        // If the player is outside the chase radius, enemy remains asleep.
        if (distance > chaseRadius) {
            animator.SetBool("isAsleep", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalkingX", false);
            animator.SetBool("isWalkingUp", false);
            animator.SetBool("isWalkingDown", false);
            hasIdled = false; // Reset so idle will trigger again on re-entry.
            return;
        }

        // When the player enters the radius, play idle animation for one second if not already done.
        if (!hasIdled) {
            StartCoroutine(IdleThenChase());
            return;
        }

        // When within chase radius (and after idling) but outside attack radius, chase the player.
        if (distance <= chaseRadius && distance > attackRadius) {
            // Calculate movement direction.
            Vector3 direction = (target.position - transform.position).normalized;
            
            // Determine and set the correct walking animation.
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                animator.SetBool("isWalkingX", true);
                animator.SetBool("isWalkingUp", false);
                animator.SetBool("isWalkingDown", false);
                // Flip the sprite based on horizontal direction (default is right).
                if(direction.x < 0) {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                } else {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
            } else {
                animator.SetBool("isWalkingX", false);
                if(direction.y > 0) {
                    animator.SetBool("isWalkingUp", true);
                    animator.SetBool("isWalkingDown", false);
                } else {
                    animator.SetBool("isWalkingUp", false);
                    animator.SetBool("isWalkingDown", true);
                }
            }
            
            // Disable asleep and idle animations when chasing.
            animator.SetBool("isAsleep", false);
            animator.SetBool("isIdle", false);

            // Move toward the player.
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    // Coroutine to handle the idle animation for one second when the player first enters the chase radius.
    IEnumerator IdleThenChase() {
        animator.SetBool("isAsleep", false);
        animator.SetBool("isIdle", true);
        animator.SetBool("isWalkingX", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("isIdle", false);
        hasIdled = true;
    }

    // This method is triggered when any collider marked as a trigger touches this enemy's collider.
    void OnTriggerEnter2D(Collider2D other) {
        // Check if the collider belongs to the player's attack collider.
        // Make sure the player's attack collider is tagged "PlayerAttack" in the editor.
        if (other.CompareTag("PlayerAttack") && !isKnocked) {
            // Calculate knockback direction: enemy is pushed away from the attack source.
            Vector2 knockDirection = (transform.position - other.transform.position).normalized;
            StartCoroutine(ApplyKnockback(knockDirection));
        }
    }

    IEnumerator ApplyKnockback(Vector2 direction) {
        isKnocked = true;

        // Apply knockback force by setting the rigidbody's velocity.
        rb.velocity = direction * knockbackForce;

        // Wait for the duration of knockback.
        yield return new WaitForSeconds(knockbackDuration);

        // Reset velocity to zero so enemy stops being pushed.
        rb.velocity = Vector2.zero;

        isKnocked = false;
    }
}
