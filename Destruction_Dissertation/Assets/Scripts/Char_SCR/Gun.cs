using UnityEngine;  
using TMPro;
using System;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bulletPrefab.GetComponent<Rigidbody>().AddForce(transform.forward * 100);
        }
    }
}