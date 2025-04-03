using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Fracture_Pipeline : MonoBehaviour
{
    public float breakRadius = .2f;
    public float breakForce = 100;

    private List<Fracture_SubFracture> cells;


    private void Start()
    {
        InitSubfractures();
    }

    private void InitSubfractures()
    {
        cells = new List<Fracture_SubFracture>();
        cells.AddRange(transform.GetComponentsInChildren<Fracture_SubFracture>());

        //Find Touching cells for every cell 
        foreach (Fracture_SubFracture cell in cells)
        {
            BoxCollider ColliderTemp = cell.gameObject.AddComponent<BoxCollider>();

            //tracking that cells are inside bounding box and if true add their connection to the subfracture list
            Collider[] hitColliders = Physics.OverlapBox(cell.transform.position, ColliderTemp.size / 2, cell.transform.rotation);
            int i = 0;

            while (i < hitColliders.Length)
            {
                if (hitColliders[i].GetComponent<Fracture_SubFracture>() && hitColliders[i].transform.root == cell.transform.root && hitColliders[i].gameObject != cell.gameObject) // Make sure that the sub fracture is part of the target object
                {
                    cell.connections.Add(hitColliders[i].GetComponent<Fracture_SubFracture>());
                    hitColliders[i].GetComponent<Fracture_SubFracture>().connections.Add(cell);
                    //Debug.Log(cell.name + "_" + hitColliders[i].name);

                }
                i++;
            }
            Destroy(ColliderTemp); // Destroy the temp colliers after first frame of tracking cells 
        }
    }

    public void Fracture(Vector3 point, Vector3 force)
    {
        foreach (Fracture_SubFracture cell in cells)
        {
            if(Vector3.Distance(cell.transform.position, point) < breakRadius)
            {
                //Checking to see if a cell is close to real time collision, if so free it from the bounding box
                cell.connections = new List<Fracture_SubFracture>();
                cell.isGround = false;
                cell.GetComponent<Rigidbody>().isKinematic = false;
                cell.GetComponent<Rigidbody>().AddForceAtPosition(force, point, ForceMode.Force);
            }
        }
    }
}
