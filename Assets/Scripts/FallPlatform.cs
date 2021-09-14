using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    
    private Rigidbody2D rb;
    void Start()
    {
          rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }
     private IEnumerator OnCollisionEnter2D(Collision2D collision)
       
        {
            yield return new WaitForSeconds(.3f);
            if(collision.gameObject.GetComponent<Rigidbody2D>())
            {
            rb.velocity = new Vector2(rb.velocity.x,-10);
            }
        }
} 
 