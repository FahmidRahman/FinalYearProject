using UnityEngine;

public class PushableRock : MonoBehaviour
{
    public string rockID = "A";
    
    public float moveDistance = 0.16f;
    public float moveSpeed = 20f;

    private bool isMoving = false;

    public void TryPush(Vector2 direction)
    {
        if (isMoving) return;

        Vector2 targetPos = (Vector2)transform.position + direction * moveDistance;

        if (!IsBlocked(targetPos))
        {
            StartCoroutine(MoveTo(targetPos));
        }
    }

    bool IsBlocked(Vector2 targetPos)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(targetPos, Vector2.one * 0.14f, 0f);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && !hit.isTrigger)
                return true;
        }
        return false;
    }

    System.Collections.IEnumerator MoveTo(Vector2 target)
    {
        isMoving = true;
        while (Vector2.Distance(transform.position, target) > 0.001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        isMoving = false;
    }
}
