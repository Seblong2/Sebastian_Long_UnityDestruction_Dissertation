using UnityEngine;
using System.Collections.Generic;
using System;

public class VoxelWorldGen : MonoBehaviour
{

    public GameObject chunkPrefab;
    public int worldSize = 5;
    public int chunkSize = 16;

    private Dictionary<Vector3Int, VoxelChunk> chunks = new Dictionary<Vector3Int, VoxelChunk>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for(int x = 0; x < worldSize; x++)
        {
            for(int z = 0; z < worldSize; z++)
            {
                Vector3Int chunkPos = new Vector3Int(x * chunkSize, 0, z * chunkSize);
                GameObject newChunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity);
                VoxelChunk chunk = newChunk.GetComponent<VoxelChunk>();

                chunks[chunkPos] = chunk;
            }
        }
    }

    public VoxelChunk GetChunk(Vector3 worldPos)
    {
        Vector3Int chunkPos = new Vector3Int(Mathf.FloorToInt(worldPos.x / chunkSize) * chunkSize,
            0,
            Mathf.FloorToInt(worldPos.z / chunkSize) * chunkSize);
        return chunks.ContainsKey(chunkPos) ? chunks[chunkPos] : null;
    }
}
