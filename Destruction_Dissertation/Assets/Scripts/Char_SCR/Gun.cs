using UnityEngine;  
using TMPro;
using System;

public class Gun : MonoBehaviour
{
    //Bullet
    public GameObject bullet;

    //BulletForce
    public float shootForce, upwardForce;

    //Gun stats (just for fun)
    public float timeBetweenShots, spread, reloadTime, timeBetweenShooting;
    public int magsize, bulletsPerClick;
    public bool allowToHold;

    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //Refs
    public Camera cam;
    public Transform attackingPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoHUD;

    //Debugging
    public bool allowInvoke = true;

    private void Awake()
    {
        //Mag is full check
        bulletsLeft = magsize;
        readyToShoot = true;

    }

    private void Update()
    {
        MyInput();
        if (ammoHUD != null)
            ammoHUD.SetText(bulletsLeft / bulletsPerClick + " / " + magsize / bulletsPerClick);
    }

    private void MyInput()
    {
        //Shooting amount dependant on weapon type
        if (allowToHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reload
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magsize && !reloading) Reload();

        //Reload automatic when mag is empty and attempting to shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //set bullet amount to 0 
            bulletsShot = 0;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        //Hit position using raycasting
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Ray in the center of viewpoint from camera
        RaycastHit hit;

        //Check for hits
        Vector3 targetPoints;
        if (Physics.Raycast(ray, out hit))

            targetPoints = hit.point;

        else
            targetPoints = ray.GetPoint(5); // Point far from player as a debug for if hits nothing

        //Calculate direction 
        Vector3 directionWithoutSpread = targetPoints - attackingPoint.position;

        //Voxel chunk destruction hit detections
        VoxelChunk chunk = null;
        if (hit.collider != null)
        {
            chunk = hit.collider.GetComponentInParent<VoxelChunk>();
        }
        if (chunk != null)
        {
            Vector3Int localVoxelPos = new Vector3Int(
                Mathf.RoundToInt(hit.point.x - chunk.transform.position.x),
                Mathf.RoundToInt(hit.point.y - chunk.transform.position.y),
                Mathf.RoundToInt(hit.point.z - chunk.transform.position.z)
                );
            chunk.DestroyVoxel(localVoxelPos);
        }

        //Spread
        float x = UnityEngine.Random.Range(-spread, spread);
        float y = UnityEngine.Random.Range(-spread, spread);

        //Calculate Direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); // Adding spread at last known direction

        //Instant bullet
        GameObject currentBullet = Instantiate(bullet, attackingPoint.position, Quaternion.identity); //Storing a bullet GameObject

        //Rotate bullet to shooting direction 
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Instantiate muzzle flash
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackingPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetshot function (if not already invoked or reset)
        if(allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = true;
        }

        if (bulletsShot < bulletsPerClick && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        //Allow shooting and invoke again for reset
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magsize;
        reloading = false;
    }
}

