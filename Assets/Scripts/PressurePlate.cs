using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool isActivated { get; private set; }

    void Update()
    {
        Collider2D rock = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Rock"));
        isActivated = rock != null;
    }
}
