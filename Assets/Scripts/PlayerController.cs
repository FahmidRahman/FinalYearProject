using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public ContactFilter2D movementFilter;
    public float collisionOffset = 0.05f;
    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Animator animator;
    SpriteRenderer spriteRenderer;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //initialising the rigidbody
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void FixedUpdate() {
        if(movementInput != Vector2.zero) {
            bool move = tryMove(movementInput);

            if (!move) {
                move = tryMove(new Vector2(movementInput.x, 0));

                if (!move) {
                    move = tryMove(new Vector2(0, movementInput.y));
                }
            }

            if (movementInput.x > 0 ) {
                animator.SetBool("isWalking", move);
                animator.SetBool("isWalkingDown", false);// Ensure isWalkingDown is false when moving horizontally
                animator.SetBool("isWalkingUp", false);
            } else if (movementInput.x < 0 ) {
                animator.SetBool("isWalking", move);
                animator.SetBool("isWalkingDown", false); // Ensure isWalkingDown is false when moving horizontally
                animator.SetBool("isWalkingUp", false);
            } else if (movementInput.y < 0 && movementInput.x == 0) {
                animator.SetBool("isWalkingDown", move);
                animator.SetBool("isWalking", false); // Ensure isWalking is false when moving down
                animator.SetBool("isWalkingUp", false);
            } else if (movementInput.y > 0 && movementInput.x ==0) {
                animator.SetBool("isWalkingUp", move);
                animator.SetBool("isWalking", false); // Ensure isWalking is false when moving down
                animator.SetBool("isWalkingDown", false);
            } else {
                animator.SetBool("isWalking", false);
                animator.SetBool("isWalkingDown", false);
                animator.SetBool("isWalkingUp", false);
            }
        }

        else {
            animator.SetBool("isWalking", false);
            animator.SetBool("isWalkingDown", false);
            animator.SetBool("isWalkingUp", false);
        }

        if (movementInput.x < 0) {
            spriteRenderer.flipX = true;
        } else if (movementInput.x > 0) {
            spriteRenderer.flipX = false;
        }
    }

    private bool tryMove(Vector2 direction){
        if (direction != Vector2.zero) {
            int count = rb.Cast(
            direction,  // X and Y values between -1 and 1 that represent the direction from the body to look for collisions.
            movementFilter,  // The settings that determine where a collision can occur on, such as layers to collide with.
            castCollisions,  // List of collisions to store the found collisions into, after the cast is finished.
            moveSpeed * Time.fixedDeltaTime + collisionOffset);  // The amount to cast equal to the movement plus an offset. Can prevent getting stuck on terrain.

            if(count == 0) {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else {
                return false;
            }
        } else {
            return false;
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

}
