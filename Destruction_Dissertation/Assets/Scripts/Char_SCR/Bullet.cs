using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float life = 3;

     void Awake()
    {
        Destroy(gameObject, life);
    }

     void OnCollisionEnter(Collision collision)
    {
        VoxelChunk chunk = collision.gameObject.GetComponent<VoxelChunk>();
        if (chunk != null)
        {
            //CONVERTING THE WORLD POSITION TO THE LOCAL VOXEL POSITION 
            Vector3 localHit = collision.contacts[0].point - chunk.transform.position;
            Vector3Int voxelPosition = new Vector3Int(
                Mathf.RoundToInt(localHit.x), 
                Mathf.RoundToInt(localHit.y), 
                Mathf.RoundToInt(localHit.z)
                );

            chunk.DestroyVoxel( voxelPosition );
        }
        
        Destroy(gameObject);
        
    }
}
