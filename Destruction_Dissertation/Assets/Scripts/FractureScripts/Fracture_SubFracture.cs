using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Fracture_SubFracture : MonoBehaviour
{
    public bool isGround;
    public bool isConnect;

    public List<Fracture_SubFracture> connections;

    private Fracture_Pipeline parent;

    private void Start()
    {
        parent = transform.root.GetComponent<Fracture_Pipeline>();
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Update()
    {
        for (int i = 0; i < connections.Count; i++) // Checking for isolation in the fractions to ensure the one that is hit isnt isolated
        {
            if (!connections[i].isGround && !connections[i].isConnect)
            {
                connections.Remove(connections[i]);
            }
        }

        bool groundedChecker = false;

        for (int i = 0; i < connections.Count; i++) //Make sure ground is connected even if it seems impossible 
        {
            if (connections[i].isGround)
            {
                groundedChecker = true;
                break;
            }

            for (int i2 = 0; i2 < connections[i].connections.Count; i2++) // Runs the iteration a second time (might remove depending on performance impact)
            {
                if (connections[i].connections[i2].isGround)
                {
                    groundedChecker = true;
                    break;
                }
            }
        }
        isConnect = groundedChecker && connections.Count >= 1 || isGround;
        GetComponent<Rigidbody>().isKinematic = isConnect;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > parent.breakForce)
        {
            //Checking if cell hit actually disconnects
            connections = new List<Fracture_SubFracture>();
            isGround = false;

            parent.Fracture(collision.contacts[0].point, collision.impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < connections.Count;i++) 
        {
            Gizmos.DrawLine(transform.position, connections[i].transform.position);
        }
    }

}
