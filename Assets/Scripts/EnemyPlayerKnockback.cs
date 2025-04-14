using UnityEngine;
using System.Collections;

public class EnemyPlayerKnockback : MonoBehaviour
{
    public float playerKnockbackForce = 10f;
    public float knockbackDuration = 0.5f;

    // Called when another collider enters this trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Calculate the knockback direction from enemy to player.
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Apply an impulse force for knockback.
                playerRb.AddForce(knockbackDirection * playerKnockbackForce, ForceMode2D.Impulse);
                PlayerHealthManager.instance.TakeDamage(1);

                // Start a coroutine to stop the knockback after the specified duration.
                StartCoroutine(EndKnockback(playerRb));
            }
        }
    }

    IEnumerator EndKnockback(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(knockbackDuration);
        // Reset the player's velocity.
        rb.velocity = Vector2.zero;
    }
}
