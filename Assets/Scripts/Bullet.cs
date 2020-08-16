using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum materialsType {BOUNCE, STATIC} 
    public PhysicsMaterial2D bounceMaterial;
    public PhysicsMaterial2D staticMaterial;
    public bool isFirstBullet = false;
    

    // Start is called before the first frame update
    void Start()
    {
        changeMaterial(materialsType.STATIC);
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

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.tag == "Trigger-waitting-bullet") {
            Camera.main.gameObject.GetComponent<Game>().NewWaittingBullet();
            gameObject.tag = "Waitting-bullet";
            gameObject.layer = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.tag == "Bumper") {
            changeMaterial(materialsType.STATIC);
            trigger.GetComponents<Bumper>()[0].bump(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Obstacle") 
        {
            other.gameObject.GetComponent<Obstacle>().Hit(1);
        }
    }

    private void OnMouseUp() {
        if (isFirstBullet) {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Camera.main.gameObject.GetComponent<Game>().ShotEvent(worldPosition);
        }
    }
}