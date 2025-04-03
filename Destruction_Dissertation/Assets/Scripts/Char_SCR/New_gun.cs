using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_gun : MonoBehaviour
{
    public Camera cam;

    public WorldGenMarching world;
 
    

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (cam.transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal")), Time.deltaTime * 10f);
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));
        cam.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        Cursor.lockState = CursorLockMode.Locked;


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //Ray in the center of viewpoint from camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                {
                    world.GetChunkFromV3(hit.transform.position).PlaceTerrain(hit.point);

                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //Ray in the center of viewpoint from camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                {
                    world.GetChunkFromV3(hit.transform.position).DestroyTerrain(hit.point);

                }
            }
        }
    }
}
