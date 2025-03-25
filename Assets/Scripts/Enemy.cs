using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public string enemyName;
    public int attackDamage;
    public float moveSpeed;
    public FloatValue maxHealth;

    private void Awake() {
        health = maxHealth.initialValue;
    }

    // Call this method to subtract damage from the enemy.
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    // Override this method in subclasses if you need additional death behavior.
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
