using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusWall : MonoBehaviour
{

    [GradientUsageAttribute(true)]
    public Gradient gradient;
    private int hp = 10;
    private int maxHp = 10;
    private Collider2D colli;
    private Renderer rend;
    public GameObject deathParticles;


    public void init(int hp)
    {
        deathParticles.SetActive(false);
        gameObject.SetActive(true);
        colli = GetComponent<Collider2D>();
        colli.enabled = true;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        this.hp = hp;
        this.maxHp = hp;
        Display();
    }

    public void Hit(int damage)
    {
        hp -= damage;
        Display();
        if (hp <= 0)
        {
            death();
        }
    }

    private void death()
    {
        colli.enabled = false;
        rend.enabled = false;
        deathParticles.SetActive(true);
    }

    private void Display()
    {
        float ratio = 1f - ((float)hp / (float)maxHp);
        Debug.Log(ratio);
        Color color = gradient.Evaluate(ratio);
        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.SetColor("_Color", color);
        renderer.material.SetVector("_EmissionColor", Color.white * 50f);
    }
}
