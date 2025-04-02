using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class MarchingManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private float heightThreshold = 0.5f;

    [SerializeField] private float noiseReduction = 1;

    [SerializeField] private bool visualizeNoise; // If i want to visual noise i will use this bool

    private float[,,] heights; // height grid Array containing the table values

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private MeshFilter meshFilter;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        StartCoroutine(UpdateAll());
    }

    private IEnumerator UpdateAll() 
    {
        while (true) 
        {
            SetMesh();
            SetHeights();
            MarchingCubes();
            yield return new WaitForSeconds(1);
        }
    }

    private void MarchingCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + Marching_Table.CubeCorners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), GetConfigureIndex(cubeCorners));
                }
            }
        }
    }

    private void MarchCube (Vector3 position, int configIndex)
    {
        if (configIndex == 0 || configIndex == 255) // Setting the boundries of marching based on the table values 0 = ground or air and 255 = ground or air
        {
            return;
        }

        int edgeIndex = 0;

        for(int t = 0; t < 5; t++)
        {
            for(int v = 0; v < 3; v++)
            {
                int triTableValue = Marching_Table.CubeTriangles[configIndex, edgeIndex];

                if(triTableValue == -1)
                {
                    return;
                }

                Vector3 edgeStart = position + Marching_Table.CubeEdges[triTableValue, 0];
                Vector3 edgeEnd = position + Marching_Table.CubeEdges[triTableValue, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    private int GetConfigureIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for(int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightThreshold)
            {
                configIndex |= 1 << i; // This operator will check for 1 inside the marching table and place a 1 if there is 0 however, if there is already one it will keep it at one and move onto the next, example 0001 0100 >> 1001 0110
            }
        }
        return configIndex;
    }

    private void SetMesh()
    {
       Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void SetHeights()
    {
        heights = new float[width + 1, height + 1, width + 1]; // Init array for heightmaps

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float currentHeight = height * Mathf.PerlinNoise(x * noiseReduction, z * noiseReduction);
                    float newHeight;

                    if (y > currentHeight) // Checking y heights and taking away the current height and assign a new height based on size
                    {
                        newHeight = y - currentHeight;
                    }
                    else
                    {
                        newHeight = currentHeight - y;
                    }
                    heights[x, y, z] = newHeight;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || !Application.isPlaying) //Checking if application is playing or if noise is visualizing 
        {
            return;
        }

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1); // Setting colour based on height values
                    Gizmos.DrawSphere(new Vector3(x, y, z), 0.2f); //Drawing spheres with range in correct positions
                }
            }
        }
    }
}
