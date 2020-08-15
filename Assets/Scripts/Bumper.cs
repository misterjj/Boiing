using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public Vector2 force;

    public bool cancelForce = true;

    public void bump(GameObject bullet) {
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (cancelForce) {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        rb.AddForce(force);
    }
}
