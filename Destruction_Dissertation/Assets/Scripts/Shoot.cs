using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;

    [System.Obsolete]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * bulletSpeed;
        }
    }
}
