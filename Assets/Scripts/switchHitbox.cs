using UnityEngine;

public class switchHitbox : MonoBehaviour
{
    
    public float leftOffset = 0.15f;
    
    private Vector3 defaultLocalPos;
    private SpriteRenderer parentSpriteRenderer;

    void Start()
    {

        defaultLocalPos = transform.localPosition;
        
        if (transform.parent != null)
        {
            parentSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (parentSpriteRenderer != null)
        {
            if (parentSpriteRenderer.flipX)
            {
                transform.localPosition = new Vector3(defaultLocalPos.x - leftOffset, defaultLocalPos.y, defaultLocalPos.z);
            }
            else
            {
                transform.localPosition = defaultLocalPos;
            }
        }
    }
}
