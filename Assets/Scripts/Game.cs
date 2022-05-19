using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject spawnPosition;
    public float spawnerRate;
    public GameObject shotPosition;
    public GameObject bulletPrefab;
    public GameObject bulletsParent;
    public int bulletCount;
    public GameObject[] posibleObstacle;
    public Transform[] posibleSpawnObstacle;
    public GameObject ObstaclesParent;
    public int maxObstaclePoint;
    public float difficultyScale;
    public GameObject levelTextValue;
    public GameObject scoreTextValue;
    public GameObject canvasGameOver;
    private int bulletInvoked = 0;
    private GameObject closestBullet;
    private Vector2 shotDirection;
    private int bulletShoted = 0;
    private int bulletWaitting = 0;
    private bool readyToShot = false;
    private bool gameOver = false;
    private int level = 0;
    private int score = 0;
    private bool projection = false;

    public float timeBeforeReSimulate = 1f;
    public float timeBeforeSinceSimulate = 0f;
   

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("InstantiateBullet", 0f, spawnerRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (projection)
        {
            GetComponent<Projection>().SimulateTrajectory(bulletPrefab, shotPosition.transform.position, shotDirection);
        }
    }

    void InstantiateBullet() 
    {
        Vector3 randomSphere = Random.insideUnitSphere;
        randomSphere.z = 0;

        var bullet = GameObject.Instantiate(bulletPrefab, spawnPosition.transform.position + randomSphere, spawnPosition.transform.rotation);
        bullet.transform.parent = bulletsParent.transform;
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
        obstacle.transform.parent = ObstaclesParent.transform;
        obstacle.GetComponent<Obstacle>().initialPoint = GetObstacleInitPoint();

        GetComponent<Projection>().UpdatePysicsScene();

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
            closestBullet.GetComponent<Bullet>().Shot(shotDirection);
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
        scoreTextValue.GetComponent<TextMesh>().text = score.ToString("0000000");
    }


    public void GameOver()
    {
        gameOver = true;
        canvasGameOver.SetActive(true);
        GameObject.Find("/Canvas Game Over/value").GetComponent<Text>().text = score.ToString("0000000");
    }

    public void Restart() 
    {
        SceneManager.LoadScene("Game");
    }
}
