using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VoxelDestruction : MonoBehaviour
{

    public GameObject mesh;

    float width;
    float height;
    float depth;

    public float Scale = 0.3f;


 
    void Start()
    {
        width = transform.localScale.z;
        height = transform.localScale.y;
        depth = transform.localScale.x;

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        mesh.gameObject.GetComponent<Transform>().localScale = new Vector3(width, height, depth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Projectile")
        {
            CreateCube();
        }
    }

    private void CreateCube()
    {
        for (float x = 0; x < width; x += Scale)
        {
            for(float y = 0; y < height; y += Scale)
            {
                for(float z = 0; z < depth; z +=Scale)
                {
                    Vector3 cubeVector = transform.position;
                    GameObject cubes = (GameObject)Instantiate(mesh, cubeVector + new Vector3(x, y, z), Quaternion.identity);
                    cubes.gameObject.GetComponent<MeshRenderer>().material = gameObject.GetComponent<MeshRenderer>().material;
                }
            }
        }
    }
}
