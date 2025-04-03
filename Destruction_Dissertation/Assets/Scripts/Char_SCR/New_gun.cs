using System;
using UnityEngine;

public class New_gun : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] float range = 10f;
    [SerializeField] private float destructionRadius = 2f;
    [SerializeField] Marching_Table marchingTable;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if(marchingTable != null)
            {
                marchingTable.DestroyTerrain(hit.point, destructionRadius);
            }
        }
    }
}
