using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPusher : MonoBehaviour
{
    public Transform pushDetector;
    private Vector2 lastMoveDir;

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input != Vector2.zero)
        {
            lastMoveDir = SnapDirection(input);

            // Move push detector in front of player
            pushDetector.localPosition = lastMoveDir * 0.16f;
        }
    }

    Vector2 SnapDirection(Vector2 raw)
    {
        if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
            return new Vector2(Mathf.Sign(raw.x), 0);
        else
            return new Vector2(0, Mathf.Sign(raw.y));
    }
}
