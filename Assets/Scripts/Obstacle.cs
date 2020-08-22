using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Gradient gradient;
    public int maxPoint;
    public int initialPoint;
    private int currentPoint;
    // Start is called before the first frame update
    void Start()
    {
        currentPoint = initialPoint;
        Display();
    }

    public void Hit(int damage = 1) 
    {
        currentPoint -= damage;
        if (currentPoint <= 0) {
            Destroy(gameObject);
        }
        Display();
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
}
