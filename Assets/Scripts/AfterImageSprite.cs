using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageSprite : MonoBehaviour
{
    private float activeTime = .1f;
    private float timeActivated;
    private float alpha;
    private float alphaSet = .8f;
    private float alphaMultiplier = .85f;

    
    private Transform character;

    private SpriteRenderer sr;
    private SpriteRenderer characterSr;
    private Color color;

    private void OnEnable() 
    {
        sr = GetComponent<SpriteRenderer>();
        character = GameObject.FindGameObjectWithTag("Player").transform;
        characterSr = character.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        sr.sprite = characterSr.sprite;
        transform.position = character.position;
        transform.rotation = character.rotation;
        timeActivated = Time.time;
    }

    private void Update() 
    {
        alpha *= alphaMultiplier;
        color = new Color(1f,1f,1f, alpha);
        sr.color = color;

        if(Time.time >= (timeActivated + activeTime))
        {
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
