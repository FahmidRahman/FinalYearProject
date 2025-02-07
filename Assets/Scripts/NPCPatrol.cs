using UnityEngine;
using System.Collections;

public class NPCPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float waitTimeAtPoint = 0.5f;

    private int currentPointIndex = 0;
    private bool waiting = false;
    private bool movingForward = true; // Controls direction of movement

    [Header("Animation Settings")]
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

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
        Vector2 direction = ((Vector2)targetPoint.position - (Vector2)transform.position).normalized;

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

        // Move forward or backward depending on current direction
        if (movingForward)
        {
            if (currentPointIndex < patrolPoints.Length - 1)
            {
                currentPointIndex++;
            }
            else
            {
                movingForward = false; // Start moving backwards
                currentPointIndex--;
            }
        }
        else
        {
            if (currentPointIndex > 0)
            {
                currentPointIndex--;
            }
            else
            {
                movingForward = true; // Start moving forward again
                currentPointIndex++;
            }
        }

        waiting = false;
    }

    void UpdateAnimation(Vector2 direction)
    {
        if (animator == null)
            return;

        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        animator.SetFloat("speed", direction.sqrMagnitude);
    }
}
