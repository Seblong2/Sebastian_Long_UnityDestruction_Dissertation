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
        int vertIndex = vertices.Count;

        //Defining corners for the cube
        Vector3[] cubeVertices = new Vector3[]
        {
            pos + new Vector3(0,0,0), //0
            pos + new Vector3(1,0,0), //1
            pos + new Vector3(1,1,0), //2
            pos + new Vector3(0,1,0), //3
            pos + new Vector3(0,0,1), //4
            pos + new Vector3(1,0,1), //5
            pos + new Vector3(1,1,1), //6
            pos + new Vector3(0,1,1), //7
        };

        //Add Vertices
        vertices.AddRange(cubeVertices);

        //Define Faces
        int[] cubeTriangles = new int[]
        {
            vertIndex, vertIndex + 1, vertIndex + 2, vertIndex, vertIndex + 2, vertIndex + 3, //FRONT
            vertIndex + 4, vertIndex + 5, vertIndex + 6, vertIndex + 4, vertIndex + 6, vertIndex +7, //BACK
            vertIndex + 0, vertIndex + 4, vertIndex + 7, vertIndex + 0, vertIndex + 7, vertIndex +3, //LEFT
            vertIndex + 1, vertIndex + 5, vertIndex + 6, vertIndex + 1, vertIndex + 6, vertIndex +2, //RIGHT
            vertIndex + 3, vertIndex + 2, vertIndex + 6, vertIndex + 3, vertIndex + 6, vertIndex +7, //TOP
            vertIndex + 0, vertIndex + 1, vertIndex + 5, vertIndex + 0, vertIndex + 5, vertIndex +4, //BOTTOM
        };

        triangles.AddRange(cubeTriangles);

        //Add uvs
        for(int i = 0; i < 8; i++)
        {
            uvs.Add(new Vector2((i % 2), (i / 4)));
        }
    }


}
