using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public Vector2 force;

    public void bump(GameObject bullet) {
        Debug.Log(bullet);
        bullet.GetComponent<Rigidbody2D>().AddForce(force);
    }
}
