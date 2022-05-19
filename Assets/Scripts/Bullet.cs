using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum materialsType {BOUNCE, STATIC} 
    public PhysicsMaterial2D bounceMaterial;
    public PhysicsMaterial2D staticMaterial;
    public bool isFirstBullet = false;
    public bool isGhost = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void changeMaterial(materialsType type) 
    {
        switch (type) {
            case materialsType.BOUNCE :
                gameObject.GetComponent<CircleCollider2D>().sharedMaterial = bounceMaterial;
                break;
            default :
                gameObject.GetComponent<CircleCollider2D>().sharedMaterial = staticMaterial;
                break;
        }
    }

    public void Shot(Vector2 shotDirection)
    {
        changeMaterial(Bullet.materialsType.BOUNCE);
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().AddForce(shotDirection * 500);
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.tag == "Trigger-waitting-bullet" && !isGhost) {
            Camera.main.gameObject.GetComponent<Game>().NewWaittingBullet();
            gameObject.tag = "Waitting-bullet";
            gameObject.layer = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.tag == "Bumper" && !isGhost) {
            changeMaterial(materialsType.STATIC);
            trigger.GetComponents<Bumper>()[0].bump(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Obstacle" && !isGhost) 
        {
            other.gameObject.GetComponent<Obstacle>().Hit(1, other);
        }
    }

    private void OnMouseUp() {
        if (isFirstBullet) {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            Camera.main.gameObject.GetComponent<Game>().ShotEvent(worldPosition);
        }
    }

    private void OnMouseDrag()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        Camera.main.gameObject.GetComponent<Game>().Simutate(worldPosition);
    }
}