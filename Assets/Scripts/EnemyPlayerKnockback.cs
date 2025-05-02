using UnityEngine;
using System.Collections;

public class EnemyPlayerKnockback : MonoBehaviour
{
    public float playerKnockbackForce = 10f;
    public float knockbackDuration = 0.5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[EnemyPlayerKnockback] Trigger entered with: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("[EnemyPlayerKnockback] Player detected!");

            PlayerHealthManager healthManager = other.GetComponent<PlayerHealthManager>();
            if (healthManager != null && healthManager.invulnerable)
            {
                Debug.Log("[EnemyPlayerKnockback] Player is invulnerable. No knockback or damage applied.");
                return;
            }

            // Knockback direction
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                Debug.Log("[EnemyPlayerKnockback] Applying knockback force.");
                playerRb.AddForce(knockbackDirection * playerKnockbackForce, ForceMode2D.Impulse);
                StartCoroutine(EndKnockback(playerRb));
            }
            else
            {
                Debug.LogWarning("[EnemyPlayerKnockback] Rigidbody2D not found on Player.");
            }

            if (healthManager != null)
            {
                Debug.Log("[EnemyPlayerKnockback] Damaging player.");
                healthManager.TakeDamage(1);
            }
            else
            {
                Debug.LogWarning("[EnemyPlayerKnockback] PlayerHealthManager not found on Player.");
            }
        }
        else
        {
            Debug.Log($"[EnemyPlayerKnockback] Ignored collision with tag: {other.tag}");
        }
    }

    IEnumerator EndKnockback(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(knockbackDuration);
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            Debug.Log("[EnemyPlayerKnockback] Knockback ended.");
        }
    }
}
