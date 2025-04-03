using System;
using UnityEngine;

public class New_gun : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] float range = 10f;
    [SerializeField] private float destructionRadius = 2f;
    [SerializeField] Marching_Table marchingTable;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f)); //Ray in the center of viewpoint from camera
            RaycastHit hit;

            
            
            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.transform.tag == "Terrain")
                {
                    hit.transform.GetComponent<Marching_Table>().DestroyTerrain(hit.point);
                    
                }
            }
    }

  
    }
}
