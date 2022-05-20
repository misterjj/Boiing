using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [GradientUsageAttribute(true)]
    public Gradient gradient;
    public int maxPoint;
    public int initialPoint;
    public GameObject rippleTemplate;
    public GameObject deathParticles;
    public float destroyTimer;
    private int currentPoint;


    // Start is called before the first frame update
    void Start()
    {
        currentPoint = initialPoint;
        Display();
    }

    public void Hit(int damage, Collision2D other) 
    {
        // spawn ripple effect
        GameObject ripple = Instantiate(rippleTemplate, other.GetContact(0).point, other.transform.rotation);
        Destroy(ripple, 0.6f);
        // calcule damage
        currentPoint -= damage;
        if (currentPoint <= 0) {
            death();
        }
        Camera.main.GetComponent<Game>().UpdateScore();
        Display();
    }

    private void death()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        deathParticles.SetActive(true);
        Destroy(gameObject, destroyTimer);
    }

    void Display()
    {
        float time = (float)currentPoint/(float)maxPoint;
        Color color = gradient.Evaluate(time);
        gameObject.GetComponent<SpriteRenderer>().color = color;
        gameObject.GetComponentInChildren<TextMesh>().text = currentPoint.ToString();

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers) {
            if (renderer.gameObject.name == "glow") {
                renderer.sharedMaterial.SetColor("_Color", color);
                renderer.material.SetVector("_EmissionColor", Color.white * 50f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Trigger-waitting-bullet") {
            Destroy(gameObject);
            Camera.main.GetComponent<Game>().GameOver();
        }    
    }
}
