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
    private int hit = 1;
    private Rigidbody2D rb;
    public int stuckFrameBeforeRelaunche = 1000;
    private int stuckFrame = 0;
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        game = Camera.main.gameObject.GetComponent<Game>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameObject.tag == "bullet" && rb.velocity == Vector2.zero)
        {
            stuckFrame++;
            if (stuckFrame >= stuckFrameBeforeRelaunche)
            {
                game.ReShotMe(this);
            }
        } else
        {
            stuckFrame = 0;
        }
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

    public void Shot(Vector2 shotDirection, int hit)
    {
        this.hit = hit;
        changeMaterial(Bullet.materialsType.BOUNCE);
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().AddForce(shotDirection * 500);
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
    }

    private void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.tag == "Bumper" && !isGhost) {
            gameObject.tag = "return-bullet";
            changeMaterial(materialsType.STATIC);
            trigger.GetComponents<Bumper>()[0].bump(gameObject);
        } else if (trigger.tag == "Trigger-waitting-bullet" && gameObject.tag != "Waitting-bullet" && !isGhost)
        {
            game.NewWaittingBullet();
            gameObject.tag = "Waitting-bullet";
            gameObject.layer = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Obstacle" && !isGhost) 
        {
            other.gameObject.GetComponent<Obstacle>().Hit(hit, other);
        }
        if (other.gameObject.tag == "Bonus-wall" && !isGhost)
        {
            other.gameObject.GetComponent<BonusWall>().Hit(hit);
        }
    }

    private void OnMouseUp() {
        if (isFirstBullet) {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            game.ShotEvent(worldPosition);
        }
    }

    private void OnMouseDrag()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        game.Simutate(worldPosition);
    }
}