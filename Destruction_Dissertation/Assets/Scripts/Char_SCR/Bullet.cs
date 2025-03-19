using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float breakForce = 10f;
    public float breakRadius = 0.2f;

    void OnCollisionEnter(Collision collision)
    {
       Fracture_Pipeline fracture_Pipeline = collision.gameObject.GetComponentInParent<Fracture_Pipeline>();
        if ( fracture_Pipeline != null )
        {
            Vector3 hitPoint = collision.contacts[0].point;

            Vector3 force = collision.impulse;

            fracture_Pipeline.Fracture( hitPoint, force );

        }

        Destroy(gameObject);
    }
}
