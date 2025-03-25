using UnityEngine;
using System.Collections;

public class EnemyPlayerKnockback : MonoBehaviour
{
    // The force to apply when knocking the player back.
    public float playerKnockbackForce = 10f;
    // How long the knockback effect lasts.
    public float knockbackDuration = 0.5f;

    // Called when another collider enters this trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the player.
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
        // Wait for the knockback duration.
        yield return new WaitForSeconds(knockbackDuration);
        // Reset the player's velocity.
        rb.velocity = Vector2.zero;
    }
}
