using UnityEngine;

public class PuzzleTile : MonoBehaviour
{
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    private TilePuzzleManager manager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = inactiveSprite;
        else
            Debug.LogError("PuzzleTile missing SpriteRenderer!", this);
    }

    public void AssignManager(TilePuzzleManager m)
    {
        manager = m;
    }

    public void ResetTile()
    {
        isActivated = false;

        // Ensure spriteRenderer is always safe to use
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer still missing during ResetTile!", this);
                return;
            }
        }

        spriteRenderer.sprite = inactiveSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (manager == null)
        {
            manager = GetComponentInParent<TilePuzzleManager>();
            if (manager == null)
            {
                Debug.LogError("PuzzleTile couldn't find its manager!", this);
                return;
            }
        }

        if (isActivated)
        {
            manager.ResetPuzzle();
        }
        else
        {
            isActivated = true;
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.sprite = activeSprite;
            manager.CheckForCompletion();
        }
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
