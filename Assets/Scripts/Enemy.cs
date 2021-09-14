using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damageDealt)
    {
        currentHealth -= damageDealt;
        anim.Play("DemonHurt");

        if (currentHealth < 0)
        {
        anim.Play("DemonDeath");
        Debug.Log("Enemy died");
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        }
        
        else
        {
        anim.Play("DemonIdle");
        }
    }

}


