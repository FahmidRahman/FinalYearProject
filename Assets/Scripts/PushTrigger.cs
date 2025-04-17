using UnityEngine;

public class PushTrigger : MonoBehaviour
{
    private PlayerPusher playerPusher;
    private PushableRock currentRock;

    void Start()
    {
        playerPusher = transform.parent.GetComponent<PlayerPusher>();
    }

    void Update()
    {
        if (currentRock != null && Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 dir = (Vector2)playerPusher.pushDetector.localPosition.normalized;
            currentRock.TryPush(dir);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock"))
        {
            currentRock = collision.GetComponent<PushableRock>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock") && currentRock != null && collision.gameObject == currentRock.gameObject)
        {
            currentRock = null;
        }
    }
}
