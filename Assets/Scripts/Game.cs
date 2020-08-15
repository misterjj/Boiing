using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject spawnPosition;
    public float spawnerRate;
    public GameObject shotPosition;
    public GameObject bulletPrefab;
    public int bulletCount;
    private int bulletInvoked = 0;
    private GameObject closestBullet;
    private Vector2 shotDirection;
    private int bulletshoted = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("InstantiateBullet", 0f, spawnerRate);
        Invoke("PrepareToShot", bulletCount * spawnerRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiateBullet() 
    {
        Vector3 randomSphere = Random.insideUnitSphere;
        randomSphere.z = 0;

        GameObject.Instantiate(bulletPrefab, spawnPosition.transform.position + randomSphere, spawnPosition.transform.rotation);
        bulletInvoked++;
        if (bulletInvoked >= bulletCount) {
            CancelInvoke("InstantiateBullet");
        }
    }

    GameObject getNextWaittingBullet() 
    {
        closestBullet = null;
        float minDist = 0f;
        GameObject[] waittingBullets = GameObject.FindGameObjectsWithTag("Waitting-bullet");

        for (int i = 0; i < waittingBullets.Length; i++) {
            float currentDist = Vector2.Distance(shotPosition.transform.position, waittingBullets[i].transform.position);
            if (closestBullet == null) {
                closestBullet = waittingBullets[i];
                minDist = currentDist;
            } else if (currentDist < minDist) {
                    closestBullet = waittingBullets[i];
                    minDist = currentDist;
            }
        }

        return closestBullet;
    }

    void PrepareToShot() {
        GameObject bullet = getNextWaittingBullet();
        bullet.transform.position = shotPosition.transform.position;
        bullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

    public void ShotEvent(Vector2 eventPosition)
    {
        bulletshoted = 0;
        Vector2 heading = eventPosition - (Vector2)shotPosition.transform.position;

        shotDirection = heading / Vector2.Distance(eventPosition, (Vector2)shotPosition.transform.position);
        InvokeRepeating("Shot", 0f, spawnerRate);
    }

    private void Shot() 
    {
        if (bulletshoted >= bulletCount) {
            CancelInvoke("Shot");
        }
        PrepareToShot();
        closestBullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        closestBullet.GetComponent<Bullet>().changeMaterial(Bullet.materialsType.BOUNCE);
        closestBullet.GetComponent<Rigidbody2D>().AddForce(shotDirection * 500);
        closestBullet.tag = "Bullet";
        closestBullet.layer = 8;
        bulletshoted++;
    }
}
