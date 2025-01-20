using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Basic_Fracture : MonoBehaviour
{
    public float breakForce = 100;
    public GameObject fractureObject;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > breakForce)
        {
            fractureObject.SetActive(true);
            fractureObject.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
