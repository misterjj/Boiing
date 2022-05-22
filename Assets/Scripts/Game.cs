using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    //spawn
    public GameObject spawnPosition;
    public float spawnerRate;

    // bullet
    public GameObject bulletPrefab;
    public GameObject bulletsParent;
    public int bulletCount;
    private int bulletInvoked = 0;
    private int bulletShoted = 0;
    private int bulletWaitting = 0;
    private GameObject closestBullet;

    // obstacle
    public GameObject[] posibleObstacle;
    public GameObject ObstaclesParent;
    public Transform[] posibleSpawnObstacle;
    public int maxObstaclePoint;
    public float difficultyScale;
    private int lastObstablePossition;

    // shot
    public GameObject shotPosition;
    private Vector2 shotDirection;
    private bool readyToShot = false;
    private bool projection = false;
    private int currentHitValue = 1;
    private int nextHitValue = 1;
    public int initialHitValue = 1;

    // UI
    public GameObject levelTextValue;
    public GameObject scoreTextValue;
    public GameObject ballsNumberTextValue;
    public GameObject currentHitTextValue;
    public GameObject nextHitTextValue;
    public GameObject canvasGameOver;
    private bool gameOver = false;
    private int level = 0;
    private int score = 0;

    // bonus
    public GameObject bonusWall;

    public void SetNextHitValue(int value)
    {
        nextHitValue = value;
        nextHitTextValue.GetComponent<TextMeshPro>().text = nextHitValue.ToString();
    }
    public int GetNextHitValue()
    {
        return nextHitValue;
    }

    public void SetActiveBonusWall(bool active = true)
    {
        bonusWall.SetActive(active);
    }


    // Start is called before the first frame update
    void Start()
    {
        SetActiveBonusWall(false);
        currentHitValue = initialHitValue;
        nextHitValue = initialHitValue;
        InvokeRepeating("InstantiateBullet", 0f, spawnerRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (projection)
        {
            GetComponent<Projection>().SimulateTrajectory(bulletPrefab, shotPosition.transform.position, shotDirection);
        }


        // shake your Rigidbody
        var x = 0f;
        var y = 0f;

        if (Mathf.Abs(Input.acceleration.x) > 1f)
        {
            x = Input.acceleration.x;
        }

        if (Mathf.Abs(Input.acceleration.z) > 1f)
        {
            y = -Input.acceleration.z * 0.25f;
        }

        Vector2 shakeVelocity = new Vector2(x, y) * 100;
        GameObject[] waittingBullets = GameObject.FindGameObjectsWithTag("Waitting-bullet");

        for (int i = 0; i < waittingBullets.Length; i++)
        {
            waittingBullets[i].GetComponent<Rigidbody2D>().AddForce(shakeVelocity * Random.Range(0.5f, 1f));
        }
    }

    public void InstantiateBullet() 
    {
        Vector3 randomSphere = Random.insideUnitSphere;
        randomSphere.z = 0;

        var bullet = GameObject.Instantiate(bulletPrefab, spawnPosition.transform.position + randomSphere, spawnPosition.transform.rotation);
        bullet.transform.parent = bulletsParent.transform;
        bulletInvoked++;

        ballsNumberTextValue.GetComponent<TextMeshPro>().text = bulletInvoked.ToString();

        if (bulletInvoked >= bulletCount) {
            CancelInvoke("InstantiateBullet");
        }
    }

    public void NewWaittingBullet() 
    {
        bulletWaitting++;
        Debug.Log("bulletWaitting" + bulletWaitting);
        Debug.Log("bulletInvoked" + bulletInvoked);
        Debug.Log("bulletWaitting == bulletInvoke" + (bulletWaitting == bulletInvoked));

        if (bulletWaitting == bulletInvoked) {
            NewLevel();
        }
    }

    private void NewLevel()
    {
        SetActiveBonusWall(false);
        UpdateScore();
        level++;
        levelTextValue.GetComponent<TextMeshPro>().text = level.ToString();

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach(GameObject obs in obstacles) {
            obs.transform.position += Vector3.up;
        }

        int newObstablePossition = Random.Range(0, posibleSpawnObstacle.Length);
        while (newObstablePossition == lastObstablePossition)
        {
            newObstablePossition = Random.Range(0, posibleSpawnObstacle.Length);
        }
        GameObject obstacleTemplate = posibleObstacle[Random.Range(0, posibleObstacle.Length)];
        Transform obstacleSpwanPosition = posibleSpawnObstacle[newObstablePossition];
        GameObject obstacle = Instantiate(obstacleTemplate, obstacleSpwanPosition.position, obstacleSpwanPosition.rotation);
        obstacle.transform.parent = ObstaclesParent.transform;
        obstacle.GetComponent<Obstacle>().initialPoint = GetObstacleInitPoint();
        lastObstablePossition = newObstablePossition;

        GetComponent<Projection>().UpdatePysicsScene();

        currentHitValue = nextHitValue;
        currentHitTextValue.GetComponent<TextMeshPro>().text = currentHitValue.ToString();
        nextHitValue = initialHitValue;
        nextHitTextValue.GetComponent<TextMeshPro>().text = nextHitValue.ToString();

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

        Debug.Log("ici" + waittingBullets.Length);

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
            closestBullet.gameObject.transform.rotation = Quaternion.identity;
            closestBullet.GetComponent<Bullet>().isFirstBullet = isFirstBullet;
            if (isFirstBullet) {
                closestBullet.GetComponent<CircleCollider2D>().enabled = false;
                closestBullet.GetComponent<BoxCollider2D>().enabled = true;
            }
            closestBullet.transform.position = shotPosition.transform.position;
            closestBullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    public void ShotEvent(Vector2 eventPosition)
    {
        if (readyToShot && !gameOver) {
            bulletWaitting = 0;
            bulletShoted = 0;
            Vector2 heading = eventPosition - (Vector2)shotPosition.transform.position;
            float distance = Vector2.Distance(eventPosition, (Vector2)shotPosition.transform.position);

            shotDirection = heading / distance;
            InvokeRepeating("Shot", 0f, spawnerRate);
            readyToShot = false;

            projection = false;
            GetComponent<Projection>().line.positionCount = 0;
        }
    }

    private void Shot() 
    {
        projection = false;
        if (bulletShoted >= bulletCount) {
            CancelInvoke("Shot");
        }
        PrepareToShot();
        if (null != closestBullet) {
            closestBullet.GetComponent<Bullet>().isFirstBullet = false;
            closestBullet.GetComponent<BoxCollider2D>().enabled = false;
            closestBullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            closestBullet.GetComponent<Bullet>().changeMaterial(Bullet.materialsType.BOUNCE);
            closestBullet.GetComponent<Bullet>().Shot(shotDirection, currentHitValue);
            closestBullet.tag = "Bullet";
            closestBullet.layer = 8;
            bulletShoted++;
        }
    }

    public void Simutate(Vector2 eventPosition)
    {
        if (readyToShot && !gameOver)
        {
            bulletWaitting = 0;
            bulletShoted = 0;
            Vector2 heading = eventPosition - (Vector2)shotPosition.transform.position;
            float distance = Vector2.Distance(eventPosition, (Vector2)shotPosition.transform.position);
            projection = true;
            shotDirection = heading / distance;
        }
    }

    public void UpdateScore()
    {
        score += level;
        scoreTextValue.GetComponent<TextMeshPro>().text = score.ToString("0000000");
    }


    public void GameOver()
    {
        gameOver = true;
        canvasGameOver.SetActive(true);
        GameObject.Find("/Canvas Game Over/value").GetComponent<Text>().text = score.ToString("0000000");
    }
}
