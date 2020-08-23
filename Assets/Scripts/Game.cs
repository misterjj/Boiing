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
    public GameObject[] posibleObstacle;
    public Transform[] posibleSpawnObstacle;
    public int maxObstaclePoint;
    public float difficultyScale;
    public GameObject levelTextValue;
    public GameObject scoreTextValue;
    private int bulletInvoked = 0;
    private GameObject closestBullet;
    private Vector2 shotDirection;
    private int bulletShoted = 0;
    private int bulletWaitting = 0;
    private bool readyToShot = false;
    private int level = 0;
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("InstantiateBullet", 0f, spawnerRate);
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

    public void NewWaittingBullet() 
    {
        bulletWaitting++;
        if (bulletWaitting == bulletCount) {
            NewLevel();
        }
    }

    private void NewLevel()
    {
        UpdateScore();
        level++;
        levelTextValue.GetComponent<TextMesh>().text = level.ToString();

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach(GameObject obs in obstacles) {
            obs.transform.position += Vector3.up;
        }

        GameObject obstacleTemplate = posibleObstacle[(int)Random.Range(0f, (float)posibleObstacle.Length)];
        Transform obstacleSpwanPosition = posibleSpawnObstacle[(int)Random.Range(0f, (float)posibleSpawnObstacle.Length)];
        GameObject obstacle = Instantiate(obstacleTemplate, obstacleSpwanPosition.position, obstacleSpwanPosition.rotation);
        obstacle.GetComponent<Obstacle>().initialPoint = GetObstacleInitPoint();

        PrepareToShot(true);
        readyToShot = true;
    }

    private int GetObstacleInitPoint() 
    {
        return (int)(maxObstaclePoint-maxObstaclePoint*Mathf.Exp(-level/difficultyScale));
    }

    void GetNextWaittingBullet() 
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
    }

    void PrepareToShot(bool isFirstBullet = false) 
    {
        GetNextWaittingBullet();
        if (null != closestBullet) {
            closestBullet.GetComponent<Bullet>().isFirstBullet = isFirstBullet;
            closestBullet.transform.position = shotPosition.transform.position;
            closestBullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    public void ShotEvent(Vector2 eventPosition)
    {
        if (readyToShot) {
            bulletWaitting = 0;
            bulletShoted = 0;
            Vector2 heading = eventPosition - (Vector2)shotPosition.transform.position;
            float distance = Vector2.Distance(eventPosition, (Vector2)shotPosition.transform.position);

            if (distance > 0.5) {
                shotDirection = heading / distance;
                InvokeRepeating("Shot", 0f, spawnerRate);
                readyToShot = false;
            }
        }
    }

    private void Shot() 
    {
        if (bulletShoted >= bulletCount) {
            CancelInvoke("Shot");
        }
        PrepareToShot();
        if (null != closestBullet) {
            closestBullet.GetComponent<Bullet>().isFirstBullet = false;
            closestBullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            closestBullet.GetComponent<Bullet>().changeMaterial(Bullet.materialsType.BOUNCE);
            closestBullet.GetComponent<Rigidbody2D>().AddForce(shotDirection * 500);
            closestBullet.tag = "Bullet";
            closestBullet.layer = 8;
            bulletShoted++;
        }
    }

    public void UpdateScore()
    {
        score += level;
        scoreTextValue.GetComponent<TextMesh>().text = score.ToString("0000000");
    }


}
