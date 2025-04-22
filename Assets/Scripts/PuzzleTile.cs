using UnityEngine;

public class PuzzleTile : MonoBehaviour
{
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    private TilePuzzleManager manager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = inactiveSprite;
        manager = Object.FindFirstObjectByType<TilePuzzleManager>();
        manager.RegisterTile(this);
    }

    public void ResetTile()
    {
        isActivated = false;
        spriteRenderer.sprite = inactiveSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (isActivated)
        {
            manager.ResetPuzzle(); // double-stepped → reset
        }
        else
        {
            isActivated = true;
            spriteRenderer.sprite = activeSprite;
            manager.CheckForCompletion(); // notify manager
        }
    }

    public bool IsActivated() => isActivated;
}
