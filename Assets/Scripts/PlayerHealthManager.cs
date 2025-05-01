using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 5;
    public int currentHearts;

    [Header("UI Settings")]
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Regeneration Settings")]
    public float regenInterval = 60f;

    [Header("Checkpoint & Player Settings")]
    public Transform checkpoint;
    public GameObject player;

    [Header("Death Settings")]
    public float deathAnimationDuration = 1f;
    public float invulnerabilityDuration = 2f;

    [HideInInspector]
    public bool invulnerable = false;

    void Start() {
        currentHearts = maxHearts;
        UpdateHeartUI();
        StartCoroutine(RegenerateHealth());
    }

    public void TakeDamage(int damage) {
        if (invulnerable)
            return;

        currentHearts -= damage;
        if (currentHearts < 0)
            currentHearts = 0;
        UpdateHeartUI();

        if (currentHearts <= 0) {
            StartCoroutine(HandlePlayerDeath());
        }
    }

    IEnumerator RegenerateHealth() {
        while (true) {
            yield return new WaitForSeconds(regenInterval);
            if (currentHearts < maxHearts) {
                currentHearts++;
                UpdateHeartUI();
            }
        }
    }

    void UpdateHeartUI() {
        for (int i = 0; i < heartImages.Length; i++) {
            heartImages[i].sprite = (i < currentHearts) ? fullHeart : emptyHeart;
        }
    }

    IEnumerator HandlePlayerDeath() {
        invulnerable = true;

        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null) {
            playerAnim.SetTrigger("player_death");
        }

        yield return new WaitForSeconds(deathAnimationDuration);

        if (playerAnim != null) {
            playerAnim.Rebind();
            playerAnim.Update(0f);
        }

        TeleportPlayerToCheckpoint();

        currentHearts = maxHearts;
        UpdateHeartUI();

        yield return new WaitForSeconds(invulnerabilityDuration);

        invulnerable = false;
    }

    void TeleportPlayerToCheckpoint() {
        if (player != null && checkpoint != null) {
            player.transform.position = checkpoint.position;
        }
    }
}
