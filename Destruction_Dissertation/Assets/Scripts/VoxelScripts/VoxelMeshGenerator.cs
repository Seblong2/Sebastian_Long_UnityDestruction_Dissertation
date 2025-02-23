using UnityEngine;
using System.Collections.Generic;
using static VoxelChunk;
using System;

public class VoxelMeshGenerator : MonoBehaviour
{
    public Mesh GenerateMesh(Voxel[,,] voxels, int chunkSize)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (voxels[x,y,z].isActive)
                    {
                        AddCubeMesh(x, y, z, vertices, triangles, uvs);
                    }
                }
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh; // Ensures a mesh is being returned
    }

    void AddCubeMesh(int x, int y, int z, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        Vector3 pos = new Vector3(x, y, z);

        vertices.Add(pos + new Vector3(0,0,1));
        vertices.Add(pos + new Vector3(0, 0, 1));
        vertices.Add(pos + new Vector3(0, 0, 1));
        vertices.Add(pos + new Vector3(0, 0, 1));

        int vertIndex = vertices.Count - 4;
        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 1);
        triangles.Add(vertIndex + 2);
        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 2);
        triangles.Add(vertIndex + 3);
    }


}
