using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyScript : Enemy
{
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform spawnPosition;

    public float knockbackDuration = 0.2f;
    public float knockbackForce = 5f;
    private bool isKnocked = false;

    private Rigidbody2D rb;
    private Animator animator;
    private bool hasIdled = false;

    private Vector2 moveTarget = Vector2.zero;
    private bool shouldMove = false;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        animator.SetBool("isIdle", false);
        animator.SetBool("isWalkingX", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
    }

    void Update()
    {
        if (!isKnocked)
        {
            CheckDistance();
        }
    }

    void FixedUpdate()
    {
        if (!isKnocked && shouldMove)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, moveTarget, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    void CheckDistance()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > chaseRadius)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalkingX", false);
            animator.SetBool("isWalkingUp", false);
            animator.SetBool("isWalkingDown", false);
            hasIdled = false;
            shouldMove = false;
            return;
        }

        if (!hasIdled)
        {
            StartCoroutine(IdleThenChase());
            shouldMove = false;
            return;
        }

        if (distance <= chaseRadius && distance > attackRadius)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                animator.SetBool("isWalkingX", true);
                animator.SetBool("isWalkingUp", false);
                animator.SetBool("isWalkingDown", false);

                if (direction.x < 0)
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                animator.SetBool("isWalkingX", false);
                if (direction.y > 0)
                {
                    animator.SetBool("isWalkingUp", true);
                    animator.SetBool("isWalkingDown", false);
                }
                else
                {
                    animator.SetBool("isWalkingUp", false);
                    animator.SetBool("isWalkingDown", true);
                }
            }

            animator.SetBool("isIdle", false);
            moveTarget = target.position;
            shouldMove = true;
        }
        else if (distance <= attackRadius)
        {
            shouldMove = false;
            rb.velocity = Vector2.zero;
        }
    }

    IEnumerator IdleThenChase()
    {
        animator.SetBool("isIdle", true);
        animator.SetBool("isWalkingX", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
        yield return new WaitForSeconds(0.01f);
        animator.SetBool("isIdle", false);
        hasIdled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack") && !isKnocked)
        {
            TakeDamage(2);

            if (health <= 0)
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = Vector2.zero;
                    }
                }
                return;
            }

            Vector2 knockDirection = (transform.position - other.transform.position).normalized;
            StartCoroutine(ApplyKnockback(knockDirection));
        }
    }

    IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnocked = true;
        rb.velocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        isKnocked = false;
    }
}
