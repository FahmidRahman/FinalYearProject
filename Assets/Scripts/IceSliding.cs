using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IceSliding : MonoBehaviour
{
    public float moveSpeed = 1f;
    public ContactFilter2D movementFilter;
    public float collisionOffset = 0.05f;
    public VectorValue startingPosition;
    public float slideSpeed = 5f;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 slideDirection;
    private bool isSliding = false;
    private bool isOnIce = false;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = startingPosition.initialValue;
    }

    private void FixedUpdate()
    {
        if (isSliding)
        {
            PerformSliding(); // Handle sliding while on ice
        }
        else if (!isOnIce) // Regular movement off the ice
        {
            if (movementInput != Vector2.zero)
            {
                bool move = tryMove(movementInput);

                if (!move)
                {
                    move = tryMove(new Vector2(movementInput.x, 0));

                    if (!move)
                    {
                        move = tryMove(new Vector2(0, movementInput.y));
                    }
                }

                HandleAnimation(move);
            }
            else
            {
                ResetAnimation();
            }

            HandleSpriteFlip();
        }
        else if (isOnIce && movementInput != Vector2.zero) // Restart sliding if on ice and input is received
        {
            slideDirection = movementInput.normalized;
            isSliding = true;
            UpdateSlidingAnimation(); // Show correct animation when sliding starts
        }
    }

    private bool tryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0) // No collision, so move
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false; // Collision detected
            }
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ice"))
        {
            isOnIce = true; // Player is on ice
            
            // Determine entry direction and force strict cardinal movement
            if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
            {
                // Coming from left or right → Force horizontal sliding
                slideDirection = new Vector2(Mathf.Sign(movementInput.x), 0);
            }
            else
            {
                // Coming from top or bottom → Force vertical sliding
                slideDirection = new Vector2(0, Mathf.Sign(movementInput.y));
            }

            // Special case: If entering ice from left or right with a downward diagonal, force horizontal
            if ((movementInput.x != 0) && (movementInput.y < 0) && Mathf.Abs(movementInput.x) >= Mathf.Abs(movementInput.y))
            {
                slideDirection = new Vector2(Mathf.Sign(movementInput.x), 0); // Force left/right
            }

            UpdateSlidingAnimation();
            ResetAnimation();
            if (slideDirection != Vector2.zero) // Start sliding immediately if input is present
            {
                isSliding = true;
            }
        }
    }

    private void PerformSliding()
    {
        if (slideDirection != Vector2.zero)
        {
            bool canSlide = tryMove(slideDirection); // Attempt to move in the slide direction

            if (!canSlide)
            {
                StopSliding(); // Stop sliding if collision occurs
            }
            else
            {
                UpdateSlidingAnimation(); // Update the sliding animation
                ResetAnimation();
            }
        }
        else
        {
            StopSliding(); // Stop sliding if direction is zero
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        StopSliding(); // Stop sliding on collision
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ice"))
        {
            isOnIce = false; // Player is no longer on ice
            StopSliding();
        }
    }

    private void StopSliding()
    {
        rb.velocity = Vector2.zero; // Stop all movement
        isSliding = false; // Reset sliding flag
        ResetAnimation();
    }

    private void HandleAnimation(bool isMoving)
    {
        if (movementInput.x > 0)
        {
            animator.SetBool("isWalking", isMoving);
            animator.SetBool("isWalkingDown", false);
            animator.SetBool("isWalkingUp", false);
        }
        else if (movementInput.x < 0)
        {
            animator.SetBool("isWalking", isMoving);
            animator.SetBool("isWalkingDown", false);
            animator.SetBool("isWalkingUp", false);
        }
        else if (movementInput.y < 0 && movementInput.x == 0)
        {
            animator.SetBool("isWalkingDown", isMoving);
            animator.SetBool("isWalking", false);
            animator.SetBool("isWalkingUp", false);
        }
        else if (movementInput.y > 0 && movementInput.x == 0)
        {
            animator.SetBool("isWalkingUp", isMoving);
            animator.SetBool("isWalking", false);
            animator.SetBool("isWalkingDown", false);
        }
    }

    private void ResetAnimation()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isWalkingDown", false);
        animator.SetBool("isWalkingUp", false);
    }

    private void HandleSpriteFlip()
    {
        if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void UpdateSlidingAnimation()
    {
        // Update animations based on the slide direction
        if (slideDirection.x > 0) // Moving right
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isWalkingDown", false);
            animator.SetBool("isWalkingUp", false);
        }
        else if (slideDirection.x < 0) // Moving left
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isWalkingDown", false);
            animator.SetBool("isWalkingUp", false);
        }
        else if (slideDirection.y < 0) // Moving down
        {
            animator.SetBool("isWalkingDown", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isWalkingUp", false);
        }
        else if (slideDirection.y > 0) // Moving up
        {
            animator.SetBool("isWalkingUp", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isWalkingDown", false);
        }

        // Adjust sprite direction for left/right
        HandleSpriteFlip();
    }

    // Mouse click handling for the attacking animation
    void Update()
    {
        if (Mouse.current.leftButton.isPressed) // Check if mouse button is clicked
        {
            if (!isAttacking) // If not already attacking, trigger attack
            {
                isAttacking = true;
                animator.SetBool("attacking", true); // Set attacking parameter to true
                StartCoroutine(ResetAttack()); // Start coroutine to reset attack after animation
            }
        }
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.3f); // Wait for attack animation to finish (adjust as necessary)
        isAttacking = false;
        animator.SetBool("attacking", false); // Reset attacking parameter
    }

}
