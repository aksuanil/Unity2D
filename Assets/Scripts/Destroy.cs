using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public GameObject Character;
    public GameObject platformPrefab;

    public GameObject fallPrefab;

    private GameObject newPlat;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
      private void OnTriggerEnter2D(Collider2D collision)
    {
      if(Random.Range(1,6) > 7)

       { 
        newPlat = (GameObject)Instantiate(platformPrefab, new Vector2(Random.Range(-5.5f,5.5f),Character.transform.position.y + (14 + Random.Range(0.5f,1f))), Quaternion.identity);
       }
      else
      {
        newPlat = (GameObject)Instantiate(fallPrefab, new Vector2(Random.Range(-5.5f,5.5f),Character.transform.position.y + (14 + Random.Range(0.5f,1f))), Quaternion.identity);
      }
      
      { 
        if (gameObject != null)
          {
            Destroy(collision.gameObject);
          }      
      }
    }
}
