using System;
using UnityEngine;

public class New_gun : MonoBehaviour
{
    public Camera cam;
 
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //Ray in the center of viewpoint from camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                {
                    hit.transform.GetComponent<Marching_Table>().DestroyTerrain(hit.point);

                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //Ray in the center of viewpoint from camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                {
                    hit.transform.GetComponent<Marching_Table>().PlaceTerrain(hit.point);

                }
            }
        }
    }
}
