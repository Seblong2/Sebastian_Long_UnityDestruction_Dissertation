using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class VoxelChunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    void Start()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunks(new Vector3(x, y, z));
                }
            }
        }

       
       MeshCreation();
    
    }

    void AddVoxelDataToChunks(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            for (int i = 0; i < 6; i++)
            {
                int triangleIndex = VoxelData.voxelTri[p, i]; // Pulling a number out of voxel data
                vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos); // using the number to add one of the vertices from the data table 
                triangles.Add(vertexIndex);//Adding to the list 

                uvs.Add(VoxelData.voxelUvs[i]);

                vertexIndex++; //Incrementation
            }
        }
    }

    void MeshCreation()
    {
        //Adding a mesh to the arrays when they are genrated
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray(); // Just a simple material adder

        mesh.RecalculateNormals(); // This just sorts the normal textures out to make sure they are facing the correct way and are placed correctly

        meshFilter.mesh = mesh;
    }

    
}
