using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bonus : MonoBehaviour
{
    protected Game game;
    public float rotateSpeed;

    private void Start()
    {
        game = Camera.main.GetComponent<Game>();
    }

    private void Update()
    {
        transform.RotateAround(transform.parent.transform.position, Vector3.forward, rotateSpeed);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, (transform.rotation.z - rotateSpeed) % 360);
    }

    abstract public void apply(); 
}
