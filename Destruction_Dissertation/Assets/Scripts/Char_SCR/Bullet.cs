using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float life = 3;

     void Awake()
    {
        Destroy(gameObject, life);
    }

     void OnCollisionEnter(Collision collision)
    {
        
        Destroy(gameObject);
        
    }
}
