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
    private bool hasIdled = false; // Flag to trigger idle animation only once when player enters the chase radius

    // Movement variables computed in Update and used in FixedUpdate
    private Vector2 moveTarget = Vector2.zero;
    private bool shouldMove = false;

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
        // Only update state if not knocked back.
        if (!isKnocked) {
            CheckDistance();
        }
    }

    // Move enemy using physics updates.
    void FixedUpdate() {
        if (!isKnocked && shouldMove) {
            // Move towards the target position computed in Update using FixedDeltaTime.
            Vector2 newPos = Vector2.MoveTowards(rb.position, moveTarget, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
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
            hasIdled = false; // Reset so idle triggers again on re-entry.
            shouldMove = false;
            return;
        }

        // When the player enters the radius, play idle animation for one second if not already done.
        if (!hasIdled) {
            StartCoroutine(IdleThenChase());
            shouldMove = false;
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
                if (direction.x < 0) {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                } else {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
            } else {
                animator.SetBool("isWalkingX", false);
                if (direction.y > 0) {
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

            // Set the moveTarget to the player's current position.
            moveTarget = target.position;
            shouldMove = true;
        }
        else if (distance <= attackRadius) {
            // When very close (within attack range), stop moving.
            shouldMove = false;
            rb.velocity = Vector2.zero;
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

    // When the enemy's trigger collider is hit by the player's attack.
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerAttack") && !isKnocked) {
            // Subtract damage.
            TakeDamage(1);
            
            // If enemy dies, cancel any knockback effect on the player.
            if (health <= 0) {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null) {
                    Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();
                    if (playerRb != null) {
                        playerRb.velocity = Vector2.zero;
                    }
                }
                // Optionally, exit here so no knockback is applied.
                return;
            }
            
            // Otherwise, apply knockback to the enemy.
            Vector2 knockDirection = (transform.position - other.transform.position).normalized;
            StartCoroutine(ApplyKnockback(knockDirection));
        }
    }

    IEnumerator ApplyKnockback(Vector2 direction) {
        isKnocked = true;
        rb.velocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        isKnocked = false;
    }
}
