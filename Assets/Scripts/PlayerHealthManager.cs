using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager instance; // Global access

    [Header("Health Settings")]
    public int maxHearts = 5;
    public int currentHearts;

    [Header("UI Settings")]
    // Array of UI Image components (hearts)
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Regeneration Settings")]
    // Time in seconds to regenerate one heart (e.g., 60 seconds = 1 minute)
    public float regenInterval = 60f;

    [Header("Checkpoint & Player Settings")]
    // Teleport location when health depletes
    public Transform checkpoint;
    // The player GameObject (should have the "Player" tag)
    public GameObject player;

    [Header("Death Settings")]
    // Duration of the death animation (in seconds)
    public float deathAnimationDuration = 1f;
    // How long the player is invulnerable after respawn (in seconds)
    public float invulnerabilityDuration = 2f;

    [HideInInspector]
    public bool invulnerable = false;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        // Start with full health.
        currentHearts = maxHearts;
        UpdateHeartUI();
        StartCoroutine(RegenerateHealth());
    }

    /// <summary>
    /// Call this method to apply damage to the player.
    /// </summary>
    public void TakeDamage(int damage) {
        if (invulnerable)
            return;

        currentHearts -= damage;
        if (currentHearts < 0)
            currentHearts = 0;
        UpdateHeartUI();

        if (currentHearts <= 0) {
            // Start the death handling coroutine
            StartCoroutine(HandlePlayerDeath());
        }
    }

    /// <summary>
    /// Regenerates one heart every regenInterval seconds until full.
    /// </summary>
    IEnumerator RegenerateHealth() {
        while (true) {
            yield return new WaitForSeconds(regenInterval);
            if (currentHearts < maxHearts) {
                currentHearts++;
                UpdateHeartUI();
            }
        }
    }

    /// <summary>
    /// Updates the heart UI images based on the current health.
    /// </summary>
    void UpdateHeartUI() {
        for (int i = 0; i < heartImages.Length; i++) {
            heartImages[i].sprite = (i < currentHearts) ? fullHeart : emptyHeart;
        }
    }

    /// <summary>
    /// Handles player death: plays death animation, waits, teleports player, resets health, and grants temporary invulnerability.
    /// </summary>
    IEnumerator HandlePlayerDeath() {
        // Make the player invulnerable to avoid additional damage
        invulnerable = true;

        // Get the player's animator and play death animation
        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null) {
            playerAnim.SetTrigger("player_death");
        }

        yield return new WaitForSeconds(deathAnimationDuration);

        // Reset the animator to default state to play idol animation
        if (playerAnim != null) {
            playerAnim.Rebind();
            playerAnim.Update(0f);
        }

        // Teleport the player to the most recent checkpoint
        TeleportPlayerToCheckpoint();

        // Reset health to max
        currentHearts = maxHearts;
        UpdateHeartUI();

        // Keep the player invulnerable for the additional invulnerabilityDuration
        yield return new WaitForSeconds(invulnerabilityDuration);

        invulnerable = false;
    }


    /// <summary>
    /// Teleports the player to the designated checkpoint.
    /// </summary>
    void TeleportPlayerToCheckpoint() {
        if (player != null && checkpoint != null) {
            player.transform.position = checkpoint.position;
        }
    }
}
