using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockEnemyScript : Enemy
{
    public  Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform spawnPosition;

    void Start() {
        target = GameObject.FindWithTag("Player").transform;

    }

    void Update() {
        CheckDistance();
    }

    void CheckDistance() {
    float distance = Vector3.Distance(target.position, transform.position);

    if (distance <= chaseRadius && distance > attackRadius) {
        // Move toward the player, but stop at attackRadius
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }
    }
}
