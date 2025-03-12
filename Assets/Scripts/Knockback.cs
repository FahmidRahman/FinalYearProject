using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    // Adjust this value to control the strength of the knockback.
    public float knockbackForce = 5f;

    // How long the enemy stays in Dynamic mode before returning to Kinematic
    public float knockbackDuration = 0.3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only apply knockback to enemies
        if(other.CompareTag("enemy"))
        {
            // Get the enemy's Rigidbody2D
            Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
            if(enemyRb != null)
            {
                enemyRb.isKinematic = false;
                Vector2 difference = enemyRb.transform.position - transform.position;
                difference = difference.normalized * knockbackForce;
                enemyRb.AddForce(difference, ForceMode2D.Impulse);
                StartCoroutine(ResetToKinematic(enemyRb));
            }
        }
    }

    private IEnumerator ResetToKinematic(Rigidbody2D enemyRb)
    {
        if(enemyRb!=null){
            yield return new WaitForSeconds(knockbackDuration);
            enemyRb.velocity = Vector2.zero;
            enemyRb.isKinematic = true;
        }
        
    }
}
