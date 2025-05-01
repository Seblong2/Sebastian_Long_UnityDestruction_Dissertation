using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_gun : MonoBehaviour
{
    public Camera cam;

    public WorldGenMarching world;
    public GameObject bulletPrefab;
    public float projectileForce = 1000f;



    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (cam.transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal")), Time.deltaTime * 10f);
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));
        cam.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        Cursor.lockState = CursorLockMode.Locked;


        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //Ray in the center of viewpoint from camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                {
                    MarchingChunk chunk = world.GetChunkFromV3(hit.transform.position);
                    if (chunk != null)
                        StartCoroutine(chunk.ApplyJob(hit.point, isPlacing: true));

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward *projectileForce, ForceMode.Impulse);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //Ray in the center of viewpoint from camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                {
                    MarchingChunk chunk = world.GetChunkFromV3(hit.transform.position);
                    if (chunk != null)
                        StartCoroutine(chunk.ApplyJob(hit.point, isPlacing: false));

                }
                if (hit.transform.CompareTag ("FractureWall"))
                {
                    Debug.Log("Wall Hit " + hit.transform.name);
                   Fracture_SubFracture fracturedPieces = hit.transform.GetComponent<Fracture_SubFracture>();

                    if (fracturedPieces != null) 
                    {
                        Vector3 forceDirection = (hit.point - hit.transform.position).normalized;
                        float forceAmount = 100f;

                        fracturedPieces.ApplyForce(forceDirection * forceAmount);

                        //fracturedPieces.transform.Translate(forceDirection * forceAmount * Time.deltaTime, Space.World);

                       // fracturedPieces.transform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.World);
                    }
                }
            }
        }
    }
}
