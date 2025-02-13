using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawn.forward * bulletSpeed;
        }
    }
}
