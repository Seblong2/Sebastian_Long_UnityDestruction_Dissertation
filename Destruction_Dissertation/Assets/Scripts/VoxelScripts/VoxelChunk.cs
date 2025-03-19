using System;
using UnityEngine;

public class VoxelChunk : MonoBehaviour
{
    public int chunkSize = 16;
    private Voxel[,,] voxels;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private VoxelMeshGenerator meshGenerator;
    public GameObject voxelprefab;

    public struct Voxel
    {
        public bool isActive;
    }

    private void Awake()
    {
        Debug.Log("Chunk awake" + gameObject.name);
        if (!gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        }

        meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

        meshCollider = gameObject.AddComponent<MeshCollider>();
        if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();

        meshGenerator = new VoxelMeshGenerator();
        voxels = new Voxel[chunkSize,chunkSize,chunkSize];

        InitializeVoxels();
        GenerateMesh();
    }

     void InitializeVoxels()
    {
       for (int x = 0; x < chunkSize; x++)
        {
            for(int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxels[x, y, z].isActive = true; //Starts all the voxels together as a solid object
                }
            }
        }
    }

    public void GenerateMesh()
    {
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter is missing" + gameObject.name);
            return;
        }

        Mesh mesh = meshGenerator.GenerateMesh(voxels, chunkSize);

        if(mesh == null || mesh.vertices.Length == 0)
        {
            Debug.LogError("Mesh Generation failed, No vertices found");
            return;
        }

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        Debug.Log("Mesh Generated with " + mesh.vertices.Length + "Vertices");
    }

    public void DestroyVoxel(Vector3Int localPosition)
    {
        if (localPosition.x >= 0 && localPosition.x < chunkSize &&
            localPosition.y >= 0 && localPosition.y < chunkSize &&
            localPosition.z >= 0 && localPosition.z < chunkSize)
        {
            voxels[localPosition.x, localPosition.y,localPosition.z].isActive = false;
            GenerateMesh();
        }
    }
    
}
