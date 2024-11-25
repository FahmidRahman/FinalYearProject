using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Transform targetPoint; // Target point for the NPC to walk to after dialogue
    public float moveSpeed = 2f; // Speed of the NPC
    private bool moveToTarget = false; // Whether the NPC should move
    private bool isPlayerNearby = false;

    private void Update()
    {
        if (moveToTarget)
        {
            MoveToTarget();
        }

        
    }

    public void StartMoving()
    {
        moveToTarget = true;
    }

    private void MoveToTarget()
    {
        if (targetPoint != null)
        {
            // Move NPC towards the target point
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            // Stop moving if close enough to the target
            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                moveToTarget = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
