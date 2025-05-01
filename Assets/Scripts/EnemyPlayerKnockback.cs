using UnityEngine;
using System.Collections;

public class EnemyPlayerKnockback : MonoBehaviour
{
    public float playerKnockbackForce = 10f;
    public float knockbackDuration = 0.5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Calculate knockback direction
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            PlayerHealthManager healthManager = other.GetComponent<PlayerHealthManager>();

            if (playerRb != null)
            {
                // Apply knockback force
                playerRb.AddForce(knockbackDirection * playerKnockbackForce, ForceMode2D.Impulse);
                StartCoroutine(EndKnockback(playerRb));
            }

            if (healthManager != null)
            {
                healthManager.TakeDamage(1);
            }
        }
    }

    IEnumerator EndKnockback(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
    }
}
